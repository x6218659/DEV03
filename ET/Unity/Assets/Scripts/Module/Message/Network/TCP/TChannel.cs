﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ETModel
{
	public class TChannel : AChannel
	{
		private readonly TcpClient tcpClient;

		private readonly CircularBuffer recvBuffer = new CircularBuffer();
		private readonly CircularBuffer sendBuffer = new CircularBuffer();

		private bool isSending;
		private readonly PacketParser parser;
		private bool isConnected;
		private TaskCompletionSource<Packet> recvTcs;

		/// <summary>
		/// connect
		/// </summary>
		public TChannel(TcpClient tcpClient, IPEndPoint ipEndPoint, TService service) : base(service, ChannelType.Connect)
		{
			this.tcpClient = tcpClient;
			this.parser = new PacketParser(this.recvBuffer);
			this.RemoteAddress = ipEndPoint;

			this.ConnectAsync(ipEndPoint);
		}

		/// <summary>
		/// accept
		/// </summary>
		public TChannel(TcpClient tcpClient, TService service) : base(service, ChannelType.Accept)
		{
			this.tcpClient = tcpClient;
			this.parser = new PacketParser(this.recvBuffer);

			IPEndPoint ipEndPoint = (IPEndPoint)this.tcpClient.Client.RemoteEndPoint;
			this.RemoteAddress = ipEndPoint;
			this.OnAccepted();
		}

		private async void ConnectAsync(IPEndPoint ipEndPoint)
		{
			try
			{
				await this.tcpClient.ConnectAsync(ipEndPoint.Address, ipEndPoint.Port);
				
				this.isConnected = true;
				this.StartSend();
				this.StartRecv();
			}
			catch (SocketException e)
			{
				Log.Error($"connect error: {e.SocketErrorCode}");
				this.OnError((int)e.SocketErrorCode);
			}
			catch (Exception e)
			{
				Log.Error($"connect error: {ipEndPoint} {e}");
				this.OnError((int)SocketError.SocketError);
			}
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			base.Dispose();

			this.recvTcs = null;
			this.tcpClient.Close();
		}

		private void OnAccepted()
		{
			this.isConnected = true;
			this.StartSend();
			this.StartRecv();
		}

		public override void Send(byte[] buffer, int index, int length)
		{
			if (this.IsDisposed)
			{
                //断线操作
                ///TODO
                Log.Warning("TChannel已经被Dispose, 不能发送消息");
                //throw new Exception("session已经被Dispose了");
                //  throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }
			byte[] size = BitConverter.GetBytes((ushort)buffer.Length);
			this.sendBuffer.Write(size, 0, size.Length);
			this.sendBuffer.Write(buffer, index, length);
			if (this.isConnected)
			{
				this.StartSend();
			}
		}

		public override void Send(List<byte[]> buffers)
		{
			if (this.IsDisposed)
			{
                //断线操作
                ///TODO
                Log.Warning("TChannel已经被Dispose, 不能发送消息");
                //throw new Exception("session已经被Dispose了");
                //throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }
			ushort size = (ushort)buffers.Select(b => b.Length).Sum();
			byte[] sizeBuffer = BitConverter.GetBytes(size);
			this.sendBuffer.Write(sizeBuffer, 0, sizeBuffer.Length);
			foreach (byte[] buffer in buffers)
			{
				this.sendBuffer.Write(buffer, 0, buffer.Length);
			}
			if (this.isConnected)
			{
				this.StartSend();
			}
		}

		private async void StartSend()
		{
			long instanceId = this.InstanceId;
			try
			{
				// 如果正在发送中,不需要再次发送
				if (this.isSending)
				{
					return;
				}

				while (true)
				{
					if (this.InstanceId != instanceId)
					{
						return;
					}

					// 没有数据需要发送
					long buffLength = this.sendBuffer.Length;
					if (buffLength == 0)
					{
						this.isSending = false;
						return;
					}

					this.isSending = true;
					
					NetworkStream stream = this.tcpClient.GetStream();
					if (!stream.CanWrite)
					{
						return;
					}

					await this.sendBuffer.WriteToAsync(stream);
				}
			}
			catch (IOException)
			{
				this.OnError((int)SocketError.SocketError);
			}
			catch (ObjectDisposedException)
			{
				this.OnError((int)SocketError.SocketError);
			}
			catch (Exception e)
			{
				Log.Error(e);
				this.OnError((int)SocketError.SocketError);
			}
		}

		private async void StartRecv()
		{
			long instanceId = this.InstanceId;
			try
			{
				while (true)
				{
					if (this.InstanceId != instanceId)
					{
						return;
					}

					NetworkStream stream = this.tcpClient.GetStream();
					if (!stream.CanRead)
					{
						return;
					}

					int n = await this.recvBuffer.ReadFromAsync(stream);

					if (n == 0)
					{
						this.OnError((int)SocketError.NetworkReset);
						return;
					}

					// 如果没有recv调用
					if (this.recvTcs == null)
					{
						continue;
					}

					try
					{
						bool isOK = this.parser.Parse();
						if (!isOK)
						{
							continue;
						}

						Packet packet = this.parser.GetPacket();

						var tcs = this.recvTcs;
						this.recvTcs = null;
						tcs.SetResult(packet);
					}
					catch (Exception e)
					{
						this.OnError(ErrorCode.ERR_PacketParserError);
						
						var tcs = this.recvTcs;
						this.recvTcs = null;
						tcs.SetException(e);
					}
				}
			}
			catch (IOException)
			{
				this.OnError((int)SocketError.SocketError);
			}
			catch (ObjectDisposedException)
			{
				this.OnError((int)SocketError.SocketError);
			}
			catch (Exception e)
			{
				Log.Error(e);
				this.OnError((int)SocketError.SocketError);
			}
		}

		public override Task<Packet> Recv()
		{
			if (this.IsDisposed)
			{
                //断线操作
                ///TODO
                Log.Warning("TChannel已经被Dispose, 不能接收消息");
                //throw new Exception("session已经被Dispose了");
                // throw new Exception("TChannel已经被Dispose, 不能接收消息");
            }

			try
			{
				bool isOK = this.parser.Parse();
				if (isOK)
				{
					Packet packet = this.parser.GetPacket();
					return Task.FromResult(packet);
				}

				this.recvTcs = new TaskCompletionSource<Packet>();
				return this.recvTcs.Task;
			}
			catch (Exception)
			{
				this.OnError(ErrorCode.ERR_PacketParserError);
				throw;
			}
		}
	}
}