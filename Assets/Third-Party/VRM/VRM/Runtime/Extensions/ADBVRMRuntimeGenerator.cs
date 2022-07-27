using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRM;

namespace ADBRuntime.Mono
{
    public static class ADBVRMRuntimeGenerator
    {
        public static void GeneratePhysics(Transform root, List<Transform> nodes,
            glTF_VRM_SecondaryAnimation secondaryAnimation)
        {

            if (secondaryAnimation.boneGroups.Count > 0)
            {
                var colliderLists = GenerateCollider(nodes, secondaryAnimation);
                GeneratePoint(root, nodes, secondaryAnimation, colliderLists);
                root.gameObject.AddComponent<CharacterPhysicsContoller>().isClone=true;
            }

        }

        private static void GeneratePoint(Transform root, List<Transform> nodes, glTF_VRM_SecondaryAnimation secondaryAnimation, List<List<ADBColliderReader>> colliderLists)
        {
            List<ADBPhysicsSetting> createSettings = new List<ADBPhysicsSetting>();
            List<ADBChainProcessor> createProcessors = new List<ADBChainProcessor>();


            for (int i = 0; i < secondaryAnimation.boneGroups.Count; i++)
            {
                var boneGroup = secondaryAnimation.boneGroups[i];

                ADBPhysicsSetting setting =
                createSettings.FirstOrDefault(setting =>
                {
                    return
                    setting.name == "Group " + i;
                });
                if (setting == null)
                {
                    setting = ScriptableObject.CreateInstance<ADBPhysicsSetting>();
                    setting.name = "Group "+i;
                    //setting.elasticityValue = Mathf.Atan(Mathf.Sqrt(boneGroup.stiffiness +4) -2) / (Mathf.PI);
                    setting.vrmStiffnessForceValue = boneGroup.stiffiness;
                    setting.dampingValue = 1 - boneGroup.dragForce;
                    setting.elasticityVelocityValue =(1 - boneGroup.dragForce) * (1 - boneGroup.dragForce);




                    setting.gravity = boneGroup.gravityDir*9.8f;
                    setting.gravityScaleValue = boneGroup.gravityPower*0.8f+0.2f;
                    setting.lengthLimitForceScaleValue = 1;
                    setting.isComputeStructuralVertical = false;
                    setting.structuralShrinkHorizontal = 0.5f;
                    setting.structuralStretchHorizontal = 1.5f;
                    setting.isComputeVirtual = false;

                }

                int colliderMask = 0;
                if (boneGroup.colliderGroups.Length > 0)
                {
                    for (int j0 = 0; j0 < boneGroup.colliderGroups.Length; j0++)
                    {
                        var colliderGroup = colliderLists[boneGroup.colliderGroups[j0]];
                        if (colliderGroup.Count!=0)
                        {
                            colliderMask |= (int)colliderGroup[0].colliderMask;
                        }
                     
                    }
                }


                for (int j0 = 0; j0 < boneGroup.bones.Length; j0++)
                {
                    Transform target = nodes[boneGroup.bones[j0]];
                    ADBRuntimePoint point = CreateADBPoint(target, "",0,colliderMask, boneGroup.hitRadius);//OYM: ������ڵ�
                    ADBChainProcessor processor = createProcessors.FirstOrDefault(x => x.CanMerge(point, setting));
                    if (processor==null)
                    {
                        processor = ADBChainProcessor.CreateADBChainProcessor(target.parent, "", setting);
                        processor.isUseLocalRadiusAndColliderMask = true;
                        createProcessors.Add(processor);
                    }
                    processor.AddChild(point);

                }
            }
            for (int i = 0; i < createProcessors.Count; i++)
            {
                createProcessors[i].Initialize();
            }

            for (int i = 0; i < createProcessors.Count; i++)
            {
                float lengthAvg=0;
                int pointCount=0;
                for (int ii = 0; ii < createProcessors[i].allPointList.Count; ii++)
                {
                    var pointData = createProcessors[i].allPointList[ii].pointRead;
                    lengthAvg += pointData.initialLocalPositionLength;
                    pointCount++;
                }
                lengthAvg /= pointCount;
                lengthAvg = 0.14f/ lengthAvg;
                var setting = createProcessors[i].GetADBSetting();

                if (lengthAvg>1f)
                {
                    setting.vrmStiffnessForceValue = Mathf.Clamp01(setting.vrmStiffnessForceValue * lengthAvg * lengthAvg);
                }
            }
            for (int i = 0; i < createProcessors.Count; i++)
            {
                createProcessors[i].Initialize();
            }

            var controller = root.gameObject.AddComponent<ADBRuntimeController>();
            controller.colliderCollisionType = ColliderCollisionType.Point;
            controller.updateMode = UpdateMode.LateUpdate;
            controller.iteration = 4;
        }

        private static ADBRuntimePoint CreateADBPoint(Transform target,string keyWord,int depth,int colliderMask,float hitRadius)
        {
            if (target == null)
            {
                return null;
            }
           var  point = ADBRuntimePoint.CreateRuntimePoint(target, depth, keyWord, !target. name.Contains(ADBChainProcessor.virtualKey));//OYM: ������ڵ�
            point.pointRead.colliderMask = colliderMask;
            point.pointRead.radius = hitRadius;
            for (int i = 0; i < target.childCount; i++)
            {
                var childPoint = CreateADBPoint(point.transform.GetChild(i), keyWord, depth + 1, colliderMask,hitRadius);
                if (childPoint != null)
                {
                    point.AddChild(childPoint);
                }
            }
            return point;
        }

        private static List<List<ADBColliderReader>> GenerateCollider(List<Transform> nodes, glTF_VRM_SecondaryAnimation secondaryAnimation)
        {
            var colliders = new List<List<ADBColliderReader>>();

            for (int i = 0; i < secondaryAnimation.colliderGroups.Count; i++)
            {
                var colliderGroup = secondaryAnimation.colliderGroups[
                    i];
                
                List<ADBColliderReader> colliderReader = new List<ADBColliderReader>();
                for (int j0 = 0; j0 < colliderGroup.colliders.Count; j0++)
                {
                    var vrmCollider = colliderGroup.colliders[j0];
                    SphereCollider unityCollider = new GameObject("colliderGroup "+i+"_"+j0).AddComponent<SphereCollider>();
                    unityCollider.transform.parent = nodes[colliderGroup.node];
                    unityCollider.transform.localPosition = vrmCollider.offset;
                    unityCollider.radius = vrmCollider.radius;
                    ADBColliderReader adbCollider = unityCollider.gameObject.AddComponent<ADBColliderReader>();
                    adbCollider.colliderMask =(ColliderChoice) (1<<i);
                    colliderReader.Add(adbCollider);
                }
                colliders.Add(colliderReader);
            }
            foreach (var colliderGroup in secondaryAnimation.colliderGroups)
            {

            }
            return colliders;
        }
    }
}

