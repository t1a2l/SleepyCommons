using ColossalFramework.UI;
using UnityEngine;

namespace SleepyCommon
{
    public class UIGroup : UIPanel
    {
        public UILabel? m_label = null;
        public UIPanel? m_content = null;

        public UILabel? label
        {
            get
            {
                return m_label;
            }
        }

        public static UIGroup? AddGroup(UIComponent parent, string text, float fTextScale, float width, float height)
        {
            UIGroup? uiGroup = null;
            if (!string.IsNullOrEmpty(text))
            {
                uiGroup = parent.AddUIComponent<UIGroup>();
                if (uiGroup is not null)
                {
                    uiGroup.padding = new RectOffset(0, 0, 2, 2);
                    uiGroup.autoLayout = true;
                    uiGroup.autoLayoutDirection = LayoutDirection.Vertical;
                    uiGroup.autoLayoutPadding = new RectOffset(4, 4, 2, 2);
                    uiGroup.autoSize = false;
                    uiGroup.width = width;
                    uiGroup.height = height;
                    uiGroup.backgroundSprite = "InfoviewPanel";
                    //uiGroup.color = new Color32(96, 96, 96, 255);
                    //uiGroup.color = new Color32(116, 116, 116, 255);
                    uiGroup.color = new Color32(82, 90, 94, 192);


                    uiGroup.m_label = uiGroup.AddUIComponent<UILabel>();
                    uiGroup.m_label.text = text;
                    uiGroup.m_label.textScale = fTextScale;
                    uiGroup.m_label.opacity = 1.0f;
                    uiGroup.m_label.padding.left = 10;

                    uiGroup.m_content = uiGroup.AddUIComponent<UIPanel>();
                    uiGroup.m_content.width = width;
                    uiGroup.m_content.autoLayout = true;
                    uiGroup.m_content.autoLayoutDirection = LayoutDirection.Vertical;
                    uiGroup.m_content.autoLayoutPadding = new RectOffset(30, 0, 0, 0);
                    uiGroup.m_content.autoSize = true;// false;
                }
            }

            return uiGroup;
        }

        public string Text
        {
            get { return m_label.text; }
            set { m_label.text = value; }
        }

        public void AdjustHeight()
        {
            if (m_label is null || m_content is null)
            {
                return;
            }

            float fHeight = m_label.height + padding.top + padding.bottom + 8;

            foreach (UIComponent component in m_content.components)
            {
                if (component.isVisible)
                {
                    fHeight += component.height + m_content.autoLayoutPadding.top + m_content.autoLayoutPadding.bottom;
                }
            }

            height = fHeight;
        }
    }
}