using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    using IK;
    using Invector.vShooter;

    [CustomEditor(typeof(vControlAIShooter), true)]
    public class vControlAIShooterEditor : vEditorBase
    {
        public vControlAIShooter shooter;
        public SerializedObject ik;
        public IKSolverEditorHelper leftIK, rightIK;
        public Transform selected, referenceSelected;
        public bool openEditor = false;
        bool openSliders;
        GUISkin _skin;
        public bool applicationStarted;

        static vControlAIShooterEditor()
        {
            EditorApplication.playmodeStateChanged -= ModeChanged;
            EditorApplication.playmodeStateChanged += ModeChanged;
        }

        static void ModeChanged()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode &&
                 EditorApplication.isPlaying)
            {
                ActiveEditorTracker.sharedTracker.isLocked = false;
            }
        }

        void DrawSceneGizmos()
        {
            if (!shooter.shooterManager) return;
            if (!shooter.CurrentActiveWeapon) return;
            applicationStarted = true;
            if (openEditor && shooter.shooterManager && shooter.shooterManager.weaponIKAdjustList && shooter.shooterManager.CurrentWeaponIK && shooter.leftIK != null && shooter.rightIK != null)
            {
                DrawIKHandles(shooter.shooterManager.CurrentWeaponIK);
            }
            try
            {
                Handles.BeginGUI();
                {
                    EditorGUILayout.BeginVertical(skin.window, GUILayout.Width(300));
                    {
                        if (GUILayout.Button(openEditor ? "Exit Shooter Editor" : "Enter Shooter Editor", _skin.button))
                        {
                            openEditor = !openEditor;
                            ActiveEditorTracker.sharedTracker.isLocked = openEditor;
                        }

                        if (openEditor)
                        {
                            if (shooter.shooterManager.weaponIKAdjustList)
                            {
                                GUILayout.BeginVertical(skin.window);
                                DrawIKOffsets(shooter.shooterManager);
                                GUILayout.EndVertical();

                                var weaponIKAdjustList = shooter.shooterManager.weaponIKAdjustList;
                                EditorGUI.BeginChangeCheck();
                                weaponIKAdjustList = (vWeaponIKAdjustList)EditorGUILayout.ObjectField(weaponIKAdjustList, typeof(vWeaponIKAdjustList), false);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    shooter.shooterManager.weaponIKAdjustList = weaponIKAdjustList;
                                    shooter.shooterManager.SetIKAdjust(weaponIKAdjustList.GetWeaponIK(shooter.shooterManager.currentWeapon.weaponCategory));
                                    return;
                                }
                                //if (shooter.shooterManager.weaponIKAdjustList.name.Equals(shooter.gameObject.name + "@IKAdjustList"))
                                //{
                                //    EditorStyles.helpBox.richText = true;
                                //    EditorGUILayout.HelpBox("<i><color=red>Please Rename this IK Ajust List</color></i>", MessageType.None);
                                //}

                                string stateTag = shooter.isCrouching ? "Crouching " : "Standing ";
                                if (shooter.isAiming) stateTag += "Aiming";
                                GUILayout.Box("State : " + stateTag + " / " + shooter.CurrentActiveWeapon.weaponCategory + " Category", skin.box, GUILayout.ExpandWidth(true));

                                if (GUILayout.Button(shooter.lockAimDebug ? "Unlock Aim" : "Lock Aim", EditorStyles.miniButton))
                                {
                                    shooter.lockAimDebug = !shooter.lockAimDebug;
                                }

                                
                                if (shooter.shooterManager.CurrentWeaponIK)
                                {
                                    if (ik == null) ik = new SerializedObject(shooter.shooterManager.CurrentWeaponIK);
                                    ik.Update();
                                    DrawWeaponIKSettings(shooter.shooterManager.CurrentWeaponIK);
                                }
                                else
                                {
                                    EditorStyles.helpBox.richText = true;
                                    EditorGUILayout.HelpBox("This weapon doesn't have a IKAdjust for the '" + shooter.CurrentActiveWeapon.weaponCategory + "' category,  click in the button below to create one.", MessageType.Info);
                                    if (GUILayout.Button("Create IK Adjust", _skin.button))
                                    {
                                        vWeaponIKAdjust ikAdjust = ScriptableObject.CreateInstance<vWeaponIKAdjust>();
                                        AssetDatabase.CreateAsset(ikAdjust, "Assets/" + shooter.gameObject.name + "@" + shooter.CurrentActiveWeapon.weaponCategory + ".asset");
                                        ikAdjust.weaponCategories = new List<string>() { shooter.CurrentActiveWeapon.weaponCategory };
                                        AssetDatabase.SaveAssets();
                                        shooter.shooterManager.weaponIKAdjustList.weaponIKAdjusts.Add(ikAdjust);
                                        shooter.shooterManager.SetIKAdjust(ikAdjust);
                                    }
                                    shooter.shooterManager.LoadIKAdjust(shooter.CurrentActiveWeapon.weaponCategory);
                                }
                            }
                            else
                            {
                                EditorStyles.helpBox.richText = true;
                                EditorStyles.helpBox.fontSize = 12;
                                EditorGUILayout.HelpBox("Your <b>ShooterManager</b> is missing a <b>IKAdjustList</b>.\n\nTo create a new list click in the button below and don't forget to <b>assign the list to your Character after exiting PlayMode</b>", MessageType.Info);

                                //EditorGUILayout.HelpBox("<color=red><b>After finish editing your IK Adjusts, don't forget to assign in the ShooterManager IK inspector when not in PlayMode</b> </color>", MessageType.Warning);

                                if (GUILayout.Button("Create IK Adjust List", _skin.button))
                                {
                                    vWeaponIKAdjustList ikAdjust = ScriptableObject.CreateInstance<vWeaponIKAdjustList>();
                                    AssetDatabase.CreateAsset(ikAdjust, "Assets/" + shooter.gameObject.name + "@IKAdjustList.asset");
                                    AssetDatabase.SaveAssets();
                                    shooter.shooterManager.weaponIKAdjustList = ikAdjust;
                                    SerializedObject scriptableIK = new SerializedObject(ikAdjust);
                                    scriptableIK.ApplyModifiedProperties();
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                Handles.EndGUI();
            }
            catch { Repaint(); }
        }

        void DrawIKHandles(vWeaponIKAdjust currentWeaponIK)
        {
            if (leftIK == null) leftIK = new IKSolverEditorHelper(shooter.leftIK);
            if (rightIK == null) rightIK = new IKSolverEditorHelper(shooter.rightIK);
            leftIK.DrawIKHandles(ref selected, ref referenceSelected, Color.blue);
            rightIK.DrawIKHandles(ref selected, ref referenceSelected, Color.green);

            if (selected != null)
            {
                if (DrawTransformHandles(selected, referenceSelected))
                {
                    Undo.RecordObject(currentWeaponIK, "Change IK Bone Transform");
                    ApplyOffsets((shooter.isAiming ? (shooter.isCrouching ? (shooter.shooterManager.CurrentWeaponIK.crouchingAiming) : shooter.shooterManager.CurrentWeaponIK.standingAiming) :
                                                   (shooter.isCrouching ? shooter.shooterManager.CurrentWeaponIK.crouching : shooter.shooterManager.CurrentWeaponIK.standing)),
                                                   shooter.shooterManager.isLeftWeapon ? shooter.leftIK : shooter.rightIK,
                                                   shooter.shooterManager.isLeftWeapon ? shooter.rightIK : shooter.leftIK);
                }
            }
        }

        void ApplyOffsets(IKAdjust currentIKAdjust, vIKSolver weaponArm, vIKSolver supportWeaponArm)
        {
            currentIKAdjust.supportHandOffset.position = supportWeaponArm.endBoneOffset.localPosition;
            currentIKAdjust.supportHandOffset.eulerAngles = supportWeaponArm.endBoneOffset.localEulerAngles;
            currentIKAdjust.supportHintOffset.position = supportWeaponArm.middleBoneOffset.localPosition;
            currentIKAdjust.supportHintOffset.eulerAngles = supportWeaponArm.middleBoneOffset.localEulerAngles;

            currentIKAdjust.weaponHandOffset.position = weaponArm.endBoneOffset.localPosition;
            currentIKAdjust.weaponHandOffset.eulerAngles = weaponArm.endBoneOffset.localEulerAngles;
            currentIKAdjust.weaponHintOffset.position = weaponArm.middleBoneOffset.localPosition;
            currentIKAdjust.weaponHintOffset.eulerAngles = weaponArm.middleBoneOffset.localEulerAngles;
            ik.ApplyModifiedProperties();
        }

        void DrawWeaponIKSettings(vWeaponIKAdjust currentWeaponIK)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(currentWeaponIK.name, _skin.box))
            {
                EditorGUIUtility.PingObject(currentWeaponIK);
            }

            if (GUILayout.Button(new GUIContent("Create Copy", "Create and Apply a copy of this IK Adjust to the current Weapon category"), _skin.button))
            {
                if (EditorUtility.DisplayDialog("Create Copy", "Create a copy of this IK Adjust? ", "Confirm"))
                {
                    string assetPath = AssetDatabase.GetAssetPath(currentWeaponIK);

                    var newAssetPath = assetPath.Replace(".asset", "_Copy.asset");
                    if (AssetDatabase.CopyAsset(assetPath, newAssetPath))
                    {
                        vWeaponIKAdjust newIK = (vWeaponIKAdjust)AssetDatabase.LoadAssetAtPath(newAssetPath, typeof(vWeaponIKAdjust));
                        shooter.shooterManager.weaponIKAdjustList.ReplaceWeaponIKAdjust(currentWeaponIK, newIK);
                        shooter.shooterManager.SetIKAdjust(newIK);
                        ProjectWindowUtil.ShowCreatedAsset(newIK);

                        return;
                    }
                }
            }

            GUILayout.EndHorizontal();
            //if (currentWeaponIK.name.Equals("NewIKAdjust"))
            //{
            //    EditorStyles.helpBox.richText = true;
            //    EditorGUILayout.HelpBox("<i><color=red>Please Rename this IK Ajust</color></i>", MessageType.None);
            //}

            GUILayout.BeginHorizontal();

            GUI.enabled = selected != shooter.leftIK.endBoneOffset;
            if (GUILayout.Button("Left Hand", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(true)))
            {
                referenceSelected = shooter.leftIK.endBone;
                selected = shooter.leftIK.endBoneOffset;
            }
            GUI.enabled = selected != shooter.leftIK.middleBoneOffset;
            if (GUILayout.Button("Left Hint", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(true)))
            {
                referenceSelected = shooter.leftIK.middleBone;
                selected = shooter.leftIK.middleBoneOffset;
            }
            GUILayout.Space(20);
            GUI.enabled = selected != shooter.rightIK.endBoneOffset;
            if (GUILayout.Button("Right Hand", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(true)))
            {
                referenceSelected = shooter.rightIK.endBone;
                selected = shooter.rightIK.endBoneOffset;
            }
            GUI.enabled = selected != shooter.rightIK.middleBoneOffset;
            if (GUILayout.Button("Right Hint", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(true)))
            {
                referenceSelected = shooter.rightIK.middleBone;
                selected = shooter.rightIK.middleBoneOffset;
            }
            GUI.enabled = true;


            GUILayout.EndHorizontal();


            if (selected != null)
            {
                GUILayout.Label(selected.name, EditorStyles.whiteBoldLabel);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Reset Position", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(true)))
                {
                    Undo.RecordObject(selected, "Reset Position");
                    selected.localPosition = Vector3.zero;
                    ApplyOffsets((shooter.isAiming ? (shooter.isCrouching ? (shooter.shooterManager.CurrentWeaponIK.crouchingAiming) : shooter.shooterManager.CurrentWeaponIK.standingAiming) :
                                              (shooter.isCrouching ? shooter.shooterManager.CurrentWeaponIK.crouching : shooter.shooterManager.CurrentWeaponIK.standing)),
                                              shooter.shooterManager.isLeftWeapon ? shooter.leftIK : shooter.rightIK,
                                              shooter.shooterManager.isLeftWeapon ? shooter.rightIK : shooter.leftIK);
                }
                if (GUILayout.Button("Reset", EditorStyles.miniButtonMid, GUILayout.ExpandWidth(true)))
                {
                    Undo.RecordObject(selected, "ResetALL");
                    selected.localPosition = Vector3.zero;
                    selected.localEulerAngles = Vector3.zero;
                    ApplyOffsets((shooter.isAiming ? (shooter.isCrouching ? (shooter.shooterManager.CurrentWeaponIK.crouchingAiming) : shooter.shooterManager.CurrentWeaponIK.standingAiming) :
                                              (shooter.isCrouching ? shooter.shooterManager.CurrentWeaponIK.crouching : shooter.shooterManager.CurrentWeaponIK.standing)),
                                              shooter.shooterManager.isLeftWeapon ? shooter.leftIK : shooter.rightIK,
                                              shooter.shooterManager.isLeftWeapon ? shooter.rightIK : shooter.leftIK);
                }
                if (GUILayout.Button("Reset Rotation", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(true)))
                {
                    Undo.RecordObject(selected, "Reset Rotation");
                    selected.localEulerAngles = Vector3.zero;
                    ApplyOffsets((shooter.isAiming ? (shooter.isCrouching ? (shooter.shooterManager.CurrentWeaponIK.crouchingAiming) : shooter.shooterManager.CurrentWeaponIK.standingAiming) :
                                              (shooter.isCrouching ? shooter.shooterManager.CurrentWeaponIK.crouching : shooter.shooterManager.CurrentWeaponIK.standing)),
                                              shooter.shooterManager.isLeftWeapon ? shooter.leftIK : shooter.rightIK,
                                              shooter.shooterManager.isLeftWeapon ? shooter.rightIK : shooter.leftIK);
                }
                GUILayout.EndHorizontal();
            }
            if (shooter.isAiming)
            {
                DrawHeadTrackSliders(shooter.isCrouching ? currentWeaponIK.crouchingAiming.spineOffset : currentWeaponIK.standingAiming.spineOffset);
            }
            else
            {
                DrawHeadTrackSliders(shooter.isCrouching ? currentWeaponIK.crouching.spineOffset : currentWeaponIK.standing.spineOffset);
            }
        }

        void DrawHeadTrackSliders(IKOffsetSpine offsetSpine)
        {
            Vector2 _spine = offsetSpine.spine;
            Vector2 _head = offsetSpine.head;
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.HelpBox("Spine / Head Offsets are applied for each state", MessageType.Info);

                GUILayout.Label("Spine Offset", EditorStyles.whiteBoldLabel);

                GUILayout.BeginHorizontal();
                DrawSlider(ref _spine.x, "X");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                DrawSlider(ref _spine.y, "Y");
                GUILayout.EndHorizontal();

                GUILayout.Label("Head Offset", EditorStyles.whiteBoldLabel);

                GUILayout.BeginHorizontal();
                DrawSlider(ref _head.x, "X");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                DrawSlider(ref _head.y, "Y");
                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(shooter.shooterManager.CurrentWeaponIK, "Change Offset Spine");
                offsetSpine.spine = _spine;
                offsetSpine.head = _head;
                ik.ApplyModifiedProperties();
            }
        }

        void DrawIKOffsets(vAIShooterManager shooterManager)
        {
            Vector3 _ikPosL = shooterManager.weaponIKAdjustList.ikPositionOffsetL;
            Vector3 _ikPosR = shooterManager.weaponIKAdjustList.ikPositionOffsetR;
            Vector3 _ikRotL = shooterManager.weaponIKAdjustList.ikRotationOffsetL;
            Vector3 _ikRotR = shooterManager.weaponIKAdjustList.ikRotationOffsetR;

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.HelpBox("Hand IK Support Global Offsets\nFirst align your support IK hand and this will work for all weapons.", MessageType.Info);

                if (!shooterManager.currentWeapon.isLeftWeapon)
                {
                    GUILayout.Label("Left IK Position Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosL.x, "X", -1, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosL.y, "Y", -1, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosL.z, "Z", -1, 1);
                    GUILayout.EndHorizontal();
                }

                if (shooterManager.currentWeapon.isLeftWeapon)
                {
                    GUILayout.Label("Right IK Position Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosR.x, "X", -1, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosR.y, "Y", -1, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosR.z, "Z", -1, 1);
                    GUILayout.EndHorizontal();
                }

                if (!shooterManager.currentWeapon.isLeftWeapon)
                {
                    GUILayout.Label("Left IK Rotation Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotL.x, "X");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotL.y, "Y");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotL.z, "Z");
                    GUILayout.EndHorizontal();
                }

                if (shooterManager.currentWeapon.isLeftWeapon)
                {
                    GUILayout.Label("Right IK Rotation Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotR.x, "X");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotR.y, "Y");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotR.z, "Z");
                    GUILayout.EndHorizontal();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(shooterManager.weaponIKAdjustList, "IkOffsets");

                shooterManager.weaponIKAdjustList.ikPositionOffsetL.x = _ikPosL.x;
                shooterManager.weaponIKAdjustList.ikPositionOffsetL.y = _ikPosL.y;
                shooterManager.weaponIKAdjustList.ikPositionOffsetL.z = _ikPosL.z;
                shooterManager.weaponIKAdjustList.ikPositionOffsetR.x = _ikPosR.x;
                shooterManager.weaponIKAdjustList.ikPositionOffsetR.y = _ikPosR.y;
                shooterManager.weaponIKAdjustList.ikPositionOffsetR.z = _ikPosR.z;
                shooterManager.weaponIKAdjustList.ikRotationOffsetL.x = _ikRotL.x;
                shooterManager.weaponIKAdjustList.ikRotationOffsetL.y = _ikRotL.y;
                shooterManager.weaponIKAdjustList.ikRotationOffsetL.z = _ikRotL.z;
                shooterManager.weaponIKAdjustList.ikRotationOffsetR.x = _ikRotR.x;
                shooterManager.weaponIKAdjustList.ikRotationOffsetR.y = _ikRotR.y;
                shooterManager.weaponIKAdjustList.ikRotationOffsetR.z = _ikRotR.z;

                ik.ApplyModifiedProperties();
            }
        }

        void DrawSlider(ref float value, string name, float min = -180, float max = 180)
        {
            GUILayout.Label(name);
            value = EditorGUILayout.Slider(value, min, max);
        }

        bool DrawTransformHandles(Transform target, Transform reference)
        {
            if (!target) return false;
            Vector3 position = target.position;
            Quaternion rotation = target.rotation;
            Handles.DrawLine(target.position, reference.position);
            if (Tools.current != Tool.Rotate)
            {
                position = Handles.PositionHandle(position, Tools.pivotRotation == PivotRotation.Local ? rotation : Quaternion.identity);
            }

            if (Tools.current == Tool.Rotate)
                rotation = Handles.RotationHandle(rotation, position);
            if (position != target.position)
            {
                Undo.RecordObject(target, "Change IK Bone Transform");
                target.position = position;

                return true;
            }
            else if (rotation != target.rotation)
            {
                Undo.RecordObject(target, "Change IK Bone Transform");
                target.rotation = rotation;
                return true;
            }
            return false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            shooter = target as vControlAIShooter;
            _skin = skin;
        }

        public void OnSceneGUI()
        {
            DrawSceneGizmos();
        }

    }
#if UNITY_EDITOR

    [System.Serializable]
    public class IKSolverEditorHelper
    {
        public vIKSolver iKSolver;
        public IKSolverEditorHelper(vIKSolver iKSolver)
        {
            this.iKSolver = iKSolver;
        }
        public void DrawIKHandles(ref Transform selected, ref Transform referenceSelected, Color color)
        {
            DrawArmLine(color);
            if (selected != iKSolver.endBoneOffset)
                if (DrawTransformButton(iKSolver.endBone, Handles.SphereHandleCap))
                {
                    referenceSelected = iKSolver.endBone;
                    selected = iKSolver.endBoneOffset;
                    Handles.color = Color.white;

                }
            if (selected != iKSolver.middleBoneOffset)
                if (DrawTransformButton(iKSolver.middleBone, Handles.CubeHandleCap))
                {
                    referenceSelected = iKSolver.middleBone;
                    selected = iKSolver.middleBoneOffset;
                    Handles.color = Color.white;
                }

            Handles.color = Color.white;
        }

        public bool DrawTransformButton(Transform target, Handles.CapFunction cap)
        {
            if (!target) return false;
            if (Handles.Button(target.position, target.rotation, 0.02f, 0.02f, cap))
            {

                return true;
            }
            return false;
        }
        void DrawArmLine(Color color)
        {
            Handles.color = color;
            Handles.DrawAAPolyLine(iKSolver.endBone.position, iKSolver.middleBone.position, iKSolver.rootBone.position);
        }
    }

#endif
}