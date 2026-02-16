//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class RemoveAxisLimits : MonoBehaviour
//{
//    [Header("Настройки")]
//    [SerializeField] private bool removeSwingLimits = true;    // Оси X и Y
//    [SerializeField] private bool removeTwistLimits = true;    // Ось Z
//    [SerializeField] private bool findOnStart = true;
//    [SerializeField] private bool findInactive = true;

//    void Start()
//    {
//        if (findOnStart)
//        {
//            RemoveAllLimits();
//        }
//    }

//    [ContextMenu("Удалить лимиты со всех CharacterJoint")]
//    public void RemoveAllLimits()
//    {
//        // Находим все CharacterJoint (включая неактивные, если указано)
//        CharacterJoint[] joints = findInactive
//            ? Resources.FindObjectsOfTypeAll<CharacterJoint>()
//            : FindObjectsOfType<CharacterJoint>();

//        int count = 0;
//        foreach (CharacterJoint joint in joints)
//        {
//            // Пропускаем префабы в редакторе
//#if UNITY_EDITOR
//            if (joint.gameObject.scene.name == null) continue;
//#endif

//            RemoveLimitsFromJoint(joint);
//            count++;
//        }

//        Debug.Log($"Удалены лимиты с {count} CharacterJoint");
//    }

//    [ContextMenu("Удалить лимиты с дочерних CharacterJoint")]
//    public void RemoveLimitsFromChildren()
//    {
//        CharacterJoint[] joints = GetComponentsInChildren<CharacterJoint>(findInactive);

//        foreach (CharacterJoint joint in joints)
//        {
//            RemoveLimitsFromJoint(joint);
//        }

//        Debug.Log($"Удалены лимиты с {joints.Length} дочерних CharacterJoint");
//    }

//    private void RemoveLimitsFromJoint(CharacterJoint joint)
//    {
//        if (joint == null) return;

//        SoftJointLimit maxLimit = new SoftJointLimit();
//        maxLimit.limit = 180f; // Максимальный угол

//        SoftJointLimit zeroLimit = new SoftJointLimit();
//        zeroLimit.limit = 0f;

//        if (removeSwingLimits)
//        {
//            // Убираем лимиты качания (оси X и Y)
//            joint.swing1Limit = maxLimit; // Swing1 - обычно ось X
//            joint.swing2Limit = maxLimit; // Swing2 - обычно ось Y
//        }

//        if (removeTwistLimits)
//        {
//            // Убираем лимиты скручивания (ось Z)
//            joint.lowTwistLimit = zeroLimit;
//            joint.highTwistLimit = zeroLimit;
//        }

//        // Настраиваем проекцию для лучшей стабильности
//        joint.enableProjection = true;
//        joint.projectionDistance = 0.1f;
//        joint.projectionAngle = 180f;

//        Debug.Log($"Лимиты удалены для {joint.name}");
//    }
//}

