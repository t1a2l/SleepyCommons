using ColossalFramework.UI;
using System;
using System.Diagnostics;
using UnityEngine;

namespace SleepyCommon
{
    public abstract class UIMainPanel<T> : UIPanel where T : UIPanel
    {
        const long iMIN_UPDATE_DELAY = 200; // ms

        private static T? s_mainPanel = null;

        // Panel updating variables
        private bool m_bUpdatePanel = false;
        private int m_panelUpdateRateMS = 4000;
        private long m_lastUdateMilliseconds = 0;
        private static Stopwatch s_stopwatch = Stopwatch.StartNew();

        // ----------------------------------------------------------------------------------------
        protected abstract void UpdatePanel();

        // ----------------------------------------------------------------------------------------
        public static T? Instance
        {
            get
            {
                if (s_mainPanel == null)
                {
                    s_mainPanel = (T) UIView.GetAView().AddUIComponent(typeof(T));
                    if (s_mainPanel is null)
                    {
                        CDebug.Log("Error: creating Panel.");
                    }
                }

                return s_mainPanel;
            }
        }

        public static bool Exists
        {
            get
            {
                return s_mainPanel != null;
            }
        }

        public static void TogglePanel()
        {
            if (Exists)
            {
                if (Instance.isVisible)
                {
                    Instance.Hide();
                }
                else
                {
                    Instance.Show();
                }
            }
            else
            {
                Instance.Show();
            }
        }

        public static bool IsVisible()
        {
            return s_mainPanel is not null && 
                    s_mainPanel.isVisible;
        }

        public static void Destroy()
        {
            if (s_mainPanel is not null)
            {
                UnityEngine.Object.Destroy(s_mainPanel.gameObject);
                s_mainPanel = null;
            }
        }

        public int PanelUpdateRate
        {
            get
            {
                return m_panelUpdateRateMS;
            }
            set
            {
                m_panelUpdateRateMS = value;
            }
        }

        public bool HandleEscape()
        {
            if (isVisible)
            {
                Hide();
                return true;
            }
            return false;
        }

        public virtual void InvalidatePanel()
        {
            m_bUpdatePanel = true;
        }

        public override void Update()
        {
            long elapsedTime = s_stopwatch.ElapsedMilliseconds;

            if ((elapsedTime - m_lastUdateMilliseconds) > iMIN_UPDATE_DELAY && isVisible)
            {
                if (m_bUpdatePanel || (PanelUpdateRate > 0 && ((elapsedTime - m_lastUdateMilliseconds) > PanelUpdateRate)))
                {
                    UpdatePanel();
                    m_bUpdatePanel = false;
                    m_lastUdateMilliseconds = s_stopwatch.ElapsedMilliseconds;
                }
            }

            base.Update();
        }

        protected void FitToScreen()
        {
            Vector2 oScreenVector = UIView.GetAView().GetScreenResolution();
            float fX = Math.Max(0.0f, Math.Min(absolutePosition.x, oScreenVector.x - width));
            float fY = Math.Max(0.0f, Math.Min(absolutePosition.y, oScreenVector.y - height));
            Vector3 oFitPosition = new Vector3(fX, fY, absolutePosition.z);
            absolutePosition = oFitPosition;
        }
    }
}