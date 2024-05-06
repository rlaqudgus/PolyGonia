#if UNITY_EDITOR
namespace DynamicLight2D
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Reflection;
    using System;

    [CustomPropertyDrawer(typeof(DropAreaAttribute))]
    public class DropAreaDrawer : PropertyDrawer
    {
       
        private DropAreaAttribute _attributeValue = null;
        private DropAreaAttribute attributeValue
        {
            get
            {
                if (_attributeValue == null)
                {
                    _attributeValue = (DropAreaAttribute)attribute;
                }

                return _attributeValue;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            DropAreaGUI(position, attributeValue.height, attributeValue.text);

            EditorUtility.SetDirty(property.serializedObject.targetObject); // Repaint

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return attributeValue.height + 50f;
        }


        bool firstTime;
        Rect dropArea;
        Color DestColor;
        string CaptionLbl = "Drop objects here";

        public void DropAreaGUI(Rect rect, float height, string Text)
        {
            Event evt = Event.current;
            Color Dragging = new Color(145/255f, 187 / 255f, 171 / 255f);
            Color Normal = new Color(219 / 255f, 219 / 255f, 219 / 255f); ;



            if (DestColor == Color.clear)
                DestColor = Normal;

            if (!firstTime) {
                firstTime = true;
                if (CaptionLbl == "") CaptionLbl = "Drop objects here";
                else CaptionLbl = attributeValue.text;

            }
           
            switch (evt.type)
            {


                case EventType.DragUpdated:

                    if (!rect.Contains(evt.mousePosition))
                    {
                        DestColor = Normal;
                        if (attributeValue.text == "") CaptionLbl = "Drop objects here";
                        else CaptionLbl = attributeValue.text;
                        return;
                    }
                        

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DestColor = Dragging;
                    CaptionLbl = DragAndDrop.objectReferences[0].ToString();


                    break;

                case EventType.DragPerform:

                    if (!rect.Contains(evt.mousePosition))
                        return;


                    DragAndDrop.AcceptDrag();

                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        runAction(DragAndDrop.objectReferences);

                    }
                    break;

                case EventType.DragExited:

                    DestColor = Normal;
                    if (attributeValue.text == "") CaptionLbl = "Drop objects here";
                    else CaptionLbl = attributeValue.text;

                    break;

                default:

                    break;


            }



           
            float space = 3f;
           

            //ALL AREA
            dropArea = new Rect(rect.x, rect.y + (space * 2), rect.width, rect.height);
            //Inner area
            Rect InnerArea = new Rect(dropArea.x + space, dropArea.y + space, dropArea.width - space*2, dropArea.height - space*2); //new Rect(rect.x + space2,( rect.y + (space2 * 2)) + space2, rect.width - space2*2, rect.height - space2*6);

            EditorGUI.DrawRect(dropArea, new Color(.8f, .8f, .8f));
            EditorGUI.DrawRect(InnerArea, DestColor);

           

            //Image
            Rect imageRect = new Rect(rect.x+ ((rect.width/2) - 32), rect.y + 10f + ((rect.height / 2) - 32), 64, 64);
            GUI.DrawTexture(imageRect, (Texture) AssetDatabase.LoadAssetAtPath(CoreUtils.MainPath() + "2DLight/Misc/drop.png", typeof(Texture)));

            // Header caption
            GUIStyle label = (GUIStyle)GUI.skin.label;
            label.normal.textColor = new Color32(100, 100, 100, 255);
            label.fontSize = 12;
            label.hover.textColor = new Color32(0, 0, 0, 255);
            Rect captionRect = new Rect((rect.x + rect.width / 2)-(CaptionLbl.Length * 3f), rect.y + 10f, rect.width,  30f);
            GUI.Label(captionRect, CaptionLbl , label);
          

           

        }

        void runAction(UnityEngine.Object[] objs)
        {
            Type t = Type.GetType(attributeValue.className);
            MethodInfo method = t.GetMethod(attributeValue.methodName, BindingFlags.Static | BindingFlags.NonPublic);
           
         
            System.Object[] parametersArray = new object[] { new object[] { (object)objs[0] } };

            method.Invoke(null, parametersArray);
        }



    }

}
#endif

