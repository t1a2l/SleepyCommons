using UnityEngine;
using ColossalFramework.UI;
using System.Collections.Generic;

namespace SleepyCommon
{
    public class UITitleBar : UIPanel
    {
        private class TitleButtonHandler
        {
            public UIButton? m_button = null;
            public MouseEventHandler? m_onClickHandler = null;
        }

        const int iTITLE_HEIGHT = 36;
        const int iBUTTON_HEIGHT = 35;
        const int iBUTTON_MARGIN = 35;

        private UISprite? m_icon = null;
        private UILabel? m_title = null;
        private UIButton? m_close = null;
        private UIDragHandle? m_drag = null;

        private List<UIButton> m_titleButtons = new List<UIButton>();
        private UIComponent? m_tooltipComponent = null;

        // ----------------------------------------------------------------------------------------
        public List<UIButton> Buttons 
        {
            get
            {
                return m_titleButtons;
            }
        }

        public bool isModal = false;
        private float m_fOffset = iBUTTON_MARGIN;

        public UITitleBar()
        {
        }

        public UIButton? closeButton
        {
            get { return m_close; }
        }

        public string title
        {
            get { return m_title?.text ?? ""; }
            set
            {
                if (m_title is not null)
                {
                    m_title.text = value;
                    m_title.position = new Vector3(this.width / 2f - m_title.width / 2f, -20f + m_title.height / 2f);
                }
            }
        }

        public UIComponent? tooltipComponent
        {
            get
            {
                return m_tooltipComponent;
            }
        }

        public static UITitleBar Create(UIPanel parent, string title, string sSpriteName, UITextureAtlas atlas, MouseEventHandler closeHandler)
        {
            UITitleBar titleBar = parent.AddUIComponent<UITitleBar>();
            titleBar.Setup(title, sSpriteName, atlas, closeHandler);    
            return titleBar;
        }

        private void Setup(string title, string sSpriteName, UITextureAtlas atlas, MouseEventHandler closeHandler)
        {
            if (parent is not null)
            {
                width = parent.width;
            }
            else
            {
                width = 200;
            }
                
            height = iTITLE_HEIGHT; 
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            relativePosition = Vector3.zero;
            backgroundSprite = "ButtonMenuDisabled";
            //backgroundSprite = "InfoviewPanel";
            //color = Color.red;

            m_icon = AddUIComponent<UISprite>();
            if (m_icon is not null)
            {
                m_icon.atlas = atlas;
                m_icon.spriteName = sSpriteName;
                m_icon.autoSize = false;
                m_icon.width = iBUTTON_HEIGHT - 4;
                m_icon.height = iBUTTON_HEIGHT - 4;
                m_icon.relativePosition = new Vector3(0.0f, 2.0f);
            }

            m_title = AddUIComponent<UILabel>();
            if (m_title is not null)
            {
                m_title.text = title;
                m_title.textAlignment = UIHorizontalAlignment.Center;
                m_title.position = new Vector3(width / 2f - m_title.width / 2f, -20f + m_title.height / 2f);
            }

            m_close = AddUIComponent<UIButton>();
            if (m_close is not null)
            {
                m_close.relativePosition = new Vector3(width - iBUTTON_MARGIN, 2);
                m_close.normalBgSprite = "buttonclose";
                m_close.hoveredBgSprite = "buttonclosehover";
                m_close.pressedBgSprite = "buttonclosepressed";
                m_close.eventClick += closeHandler;

                m_fOffset += m_close.width;
            }
        }

        public int AddButton(string name, UITextureAtlas atlas, string spriteName, string sTooltip, MouseEventHandler handler)
        {
            UIButton button = AddUIComponent<UIButton>();
            if (button is not null)
            {
                button.name = name;
                button.tooltip = sTooltip;
                button.width = iBUTTON_HEIGHT;
                button.height = iBUTTON_HEIGHT;
                button.relativePosition = new Vector3(width - m_fOffset, 2);
                button.atlas = atlas;
                button.normalBgSprite = spriteName;
                button.color = Color.white;
                button.eventClick += handler;
                button.eventTooltipEnter += OnTooltipEnter;
                button.eventTooltipLeave += OnTooltipLeave;

                m_titleButtons.Add(button);
                m_fOffset += button.width;

                return m_titleButtons.Count - 1;
            }

            return -1;
        }

        public void SetupButtons(DragEventHandler dragHandler = null)
        {
            m_drag = AddUIComponent<UIDragHandle>();
            if (m_drag is not null)
            {
                m_drag.width = width - m_fOffset;
                m_drag.height = height;
                m_drag.relativePosition = Vector3.zero;
                m_drag.target = parent;

                if (dragHandler != null)
                {
                    m_drag.eventDragStart += dragHandler;
                }
                
            }
        }

        public void OnTooltipEnter(UIComponent component, UIMouseEventParameter eventParam) 
        {
            m_tooltipComponent = component;
        }
        public void OnTooltipLeave(UIComponent component, UIMouseEventParameter eventParam)
        {
            m_tooltipComponent = null;
        }

        public override void OnDestroy()
        {
            if (m_icon is not null)
            {
                Destroy(m_icon.gameObject);
            }
            if (m_title is not null)
            {
                Destroy(m_title.gameObject);
            }
            if (m_close is not null)
            {
                Destroy(m_close.gameObject);
            }
            if (m_drag is not null)
            {
                Destroy(m_drag.gameObject);
            }

            base.OnDestroy();
        }
    }
}