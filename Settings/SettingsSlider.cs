using ColossalFramework.UI;
using System;
using UnityEngine;

namespace SleepyCommon
{
	public class SettingsSlider : UIPanel
    {
		const int iSLIDER_PANEL_HEIGHT = 34;

		public UILabel? m_label = null;
		public int m_iDecimalPlaces = 0;

		private UISlider? m_slider = null;
		private string m_sText = "";
		private float m_fValue;
		private float m_fOffValue = float.MaxValue;
		private ICities.OnValueChanged? m_eventCallback = null;
		private bool m_bPercent = false;

		public float Value
		{
			get
			{
				return m_fValue;
			}
			set
			{
				m_fValue = value;
                SetValue(m_fValue);
            }
		}

        public string Text
        {
            get
            {
                return m_sText;
            }
            set
            {
                m_sText = value;
                UpdateLabel();
            }
        }

        public float OffValue
        {
            get
            {
                return m_fOffValue;
            }
            set
            {
                m_fOffValue = value;
                UpdateLabel();
            }
        }

        public bool Percent
		{
			get 
			{ 
				return m_bPercent; 
			}
			set
			{
				m_bPercent = value;
                UpdateLabel();
			}
		}

		// Uses the default settings font
        public static SettingsSlider CreateSettingsStyle(UIHelper helper, LayoutDirection direction, string sText, int iLabelWidth, int iSliderWidth, float fMin, float fMax, float fStep, float fDefault, int iDecimalPlaces, ICities.OnValueChanged eventCallback)
        {
            UIFont defaultFont = UIFonts.Regular;
			defaultFont.size = 16;
            return Create((UIPanel)helper.self, direction, sText, defaultFont, 1.125f, iLabelWidth, iSliderWidth, fMin, fMax, fStep, fDefault, iDecimalPlaces, eventCallback);
        }

        public static SettingsSlider Create(UIHelper helper, LayoutDirection direction, string sText, UIFont font, float fTextScale, int iLabelWidth, int iSliderWidth, float fMin, float fMax, float fStep, float fDefault, int iDecimalPlaces, ICities.OnValueChanged eventCallback)
		{
            return Create((UIPanel)helper.self, direction, sText, font, fTextScale, iLabelWidth, iSliderWidth, fMin, fMax, fStep, fDefault, iDecimalPlaces, eventCallback);
        }

        public static SettingsSlider Create(UIPanel parent, LayoutDirection direction, string sText, UIFont font, float fTextScale, int iLabelWidth, int iSliderWidth, float fMin, float fMax, float fStep, float fDefault, int iDecimalPlaces, ICities.OnValueChanged eventCallback)
		{
			SettingsSlider oSlider = parent.AddUIComponent<SettingsSlider>();
			oSlider.Setup(parent, direction, sText, font, fTextScale, iLabelWidth, iSliderWidth, fMin, fMax, fStep, fDefault, iDecimalPlaces, eventCallback);
            return oSlider;
		}

        public void Setup(UIPanel parent, LayoutDirection direction, string sText, UIFont font, float fTextScale, int iLabelWidth, int iSliderWidth, float fMin, float fMax, float fStep, float fDefault, int iDecimalPlaces, ICities.OnValueChanged eventCallback)
        {
            // Panel
            autoLayout = true;
            autoLayoutDirection = direction;
            padding.top = 6;
            autoSize = false;
            width = iLabelWidth + iSliderWidth + 10;
            height = (direction == LayoutDirection.Horizontal) ? iSLIDER_PANEL_HEIGHT : 2 * iSLIDER_PANEL_HEIGHT;
            //oSlider.m_panel.backgroundSprite = "InfoviewPanel";
            //oSlider.m_panel.color = Color.red;

            m_fValue = fDefault;
            m_eventCallback = eventCallback;
            m_iDecimalPlaces = iDecimalPlaces;
            m_sText = sText;

            // Label
            m_label = AddUIComponent<UILabel>();
            if (m_label is not null)
            {
                m_label.autoSize = false;
                m_label.width = (direction == LayoutDirection.Vertical) ? width : iLabelWidth;
                m_label.height = 28;
                m_label.text = m_sText + ": " + fDefault;
                m_label.font = font;
                m_label.textScale = fTextScale;
                //oSlider.m_label.backgroundSprite = "InfoviewPanel";
                //oSlider.m_label.color = Color.blue;
            }

            // Slider
            CreateSlider((direction == LayoutDirection.Vertical) ? width : iSliderWidth, 30, fMin, fMax);
            m_slider.value = (float)Math.Round(fDefault, m_iDecimalPlaces);
            m_slider.stepSize = fStep;
            m_slider.eventValueChanged += delegate (UIComponent c, float val)
            {
                OnSliderValueChanged(val);
            };

            // Replace slider with a new one
            GameObject.Destroy(m_slider.thumbObject.gameObject);
            UISprite thumb = m_slider.AddUIComponent<UISprite>();
            thumb.size = new Vector2(16, 16);
            thumb.position = new Vector2(0, 0);
            thumb.spriteName = "InfoIconBaseHovered";
            m_slider.thumbObject = thumb;

            UpdateLabel();
        }

        private void CreateSlider(float width, float height, float min, float max)
		{
			UIPanel bg = AddUIComponent<UIPanel>();
			bg.name = "sliderPanel";
			bg.autoLayout = false;
			bg.padding = new RectOffset(0, 0, 10, 0);
			bg.size = new Vector2(width, height);
			//bg.backgroundSprite = "InfoviewPanel";
			//bg.color = Color.green;

			m_slider = bg.AddUIComponent<UISlider>();
            m_slider.autoSize = false;
            m_slider.area = new Vector4(8, 0, bg.width, 15);
            m_slider.width = bg.width - 10;
            m_slider.height = 1;
            m_slider.relativePosition = new Vector3(8, m_slider.relativePosition.y + 10);
            m_slider.backgroundSprite = "SubBarButtonBasePressed";
            m_slider.fillPadding = new RectOffset(6, 6, 0, 0);
            m_slider.maxValue = max;
            m_slider.minValue = min;

			UISprite thumb = m_slider.AddUIComponent<UISprite>();
			thumb.size = new Vector2(16, 16);
			thumb.position = new Vector2(0, 0);
			thumb.spriteName = "InfoIconBaseHovered";

            m_slider.value = 0.0f;
            m_slider.thumbObject = thumb;
		}

		public void SetTooltip(string sTooltip)
		{
			if (m_slider is not null)
			{
				m_slider.tooltip = sTooltip;
			}
		}

		public void OnSliderValueChanged(float fValue)
        {
			m_fValue = fValue;
			UpdateLabel();
			if (m_eventCallback is not null)
            {
				m_eventCallback(fValue);
			}
		}

		private void UpdateLabel()
		{
            if (m_label is not null)
            {
				string sLabel = string.Empty;
				if (m_fValue == OffValue)
				{
                    sLabel = $"{m_sText}: Off";
                }
				else
				{
                    sLabel = $"{m_sText}: {m_fValue.ToString($"N{m_iDecimalPlaces}")}{(Percent ? "%" : "")}";
                }

				m_label.text = sLabel;
            }
        }

		public void Enable(bool bEnable)
		{
			if (m_label is not null)
            {
				m_label.isEnabled = bEnable;
				m_label.disabledTextColor = Color.grey;
			}
			if (m_slider is not null)
            {
				m_slider.isEnabled = bEnable;
			}
		}

		private void SetValue(float fValue)
        {
			if (m_slider is not null)
			{
				m_slider.value = fValue;
			}
		}

		public void Destroy()
        {
			if (m_label is not null)
            {
				UnityEngine.Object.Destroy(m_label.gameObject);
				m_label = null;

			}
			if (m_slider is not null)
			{
				UnityEngine.Object.Destroy(m_slider.gameObject);
				m_slider = null;
			}
		}
	}

    
}
