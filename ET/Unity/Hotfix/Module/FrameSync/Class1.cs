using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class ClassComponentAwakeSystem : AwakeSystem<ClassComponent>
    {
        public override void Awake(ClassComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class ClassComponentUpdateSystem : UpdateSystem<ClassComponent>
    {
        public override void Update(ClassComponent self)
        {
            self.Update();
        }
    }

    public class ClassComponent : Component
    {
        public void Awake()
        {
            
        }
        public void Update()
        {
            Debug.Log("23");
        }

    }

}
