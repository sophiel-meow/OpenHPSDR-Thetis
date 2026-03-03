//=================================================================
// LanguageManager.cs
//=================================================================
// Language management for Thetis multi-language support
// Supports runtime language switching between English and Chinese
//=================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace Thetis
{
    public static class LanguageManager
    {
        private static CultureInfo currentCulture = new CultureInfo("zh-CN");

        static LanguageManager()
        {
            Thread.CurrentThread.CurrentUICulture = currentCulture;
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        // Stores original (English) text for each control, keyed weakly so GC can collect closed forms
        private static readonly ConditionalWeakTable<Control, StrongBox<string>> _originalTexts =
            new ConditionalWeakTable<Control, StrongBox<string>>();

        public static event EventHandler LanguageChanged;

        public static CultureInfo CurrentCulture
        {
            get { return currentCulture; }
        }

        public static void SetLanguage(string cultureName)
        {
            CultureInfo newCulture = new CultureInfo(cultureName);
            if (currentCulture.Name != newCulture.Name)
            {
                currentCulture = newCulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;
                Thread.CurrentThread.CurrentCulture = newCulture;

                LanguageChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        // -------------------------------------------------------
        // Resource-based translation (for console form which uses .resx)
        // -------------------------------------------------------

        public static void ApplyLanguage(Form form)
        {
            ComponentResourceManager resources = new ComponentResourceManager(form.GetType());
            ApplyResourceToControl(form, resources);
        }

        private static void ApplyResourceToControl(Control control, ComponentResourceManager resources)
        {
            resources.ApplyResources(control, control.Name);

            foreach (Control child in control.Controls)
            {
                ApplyResourceToControl(child, resources);
            }

            // Handle MenuStrip separately
            if (control is Form form)
            {
                foreach (Control ctrl in form.Controls)
                {
                    if (ctrl is MenuStrip menuStrip)
                    {
                        ApplyResourceToMenuStrip(menuStrip, resources);
                    }
                }
            }
        }

        private static void ApplyResourceToMenuStrip(MenuStrip menuStrip, ComponentResourceManager resources)
        {
            resources.ApplyResources(menuStrip, menuStrip.Name);

            foreach (ToolStripItem item in menuStrip.Items)
            {
                ApplyResourceToToolStripItem(item, resources);
            }
        }

        private static void ApplyResourceToToolStripItem(ToolStripItem item, ComponentResourceManager resources)
        {
            resources.ApplyResources(item, item.Name);

            if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
            {
                foreach (ToolStripItem dropDownItem in menuItem.DropDownItems)
                {
                    ApplyResourceToToolStripItem(dropDownItem, resources);
                }
            }
        }

        // -------------------------------------------------------
        // Programmatic translation (for forms with hardcoded text)
        // -------------------------------------------------------

        /// <summary>
        /// Register a form and apply the current language's translations.
        /// Call this in Form_Load for any form that needs translation support.
        /// </summary>
        public static void RegisterAndTranslateForm(Form form)
        {
            // Store original English texts (only if not already stored)
            StoreOriginalTexts(form);
            // Apply current language
            if (currentCulture.Name != "en-US")
                ApplyTranslationsToControl(form);
        }

        /// <summary>
        /// Apply current language translations to a form (for use in LanguageChanged handler).
        /// </summary>
        public static void TranslateForm(Form form)
        {
            if (currentCulture.Name == "en-US")
                RestoreOriginalTexts(form);
            else
                ApplyTranslationsToControl(form);
        }

        private static void StoreOriginalTexts(Control control)
        {
            // Only store if not already in the table
            if (!_originalTexts.TryGetValue(control, out _))
            {
                _originalTexts.Add(control, new StrongBox<string>(control.Text ?? ""));
            }

            foreach (Control child in control.Controls)
                StoreOriginalTexts(child);
        }

        private static void ApplyTranslationsToControl(Control control)
        {
            // Get the stored original English text
            if (_originalTexts.TryGetValue(control, out StrongBox<string> box))
            {
                string original = box.Value;
                if (!string.IsNullOrEmpty(original))
                {
                    string translated = Translations.GetTranslation(original, currentCulture.Name);
                    if (control.Text != translated)
                        control.Text = translated;
                }
            }

            foreach (Control child in control.Controls)
                ApplyTranslationsToControl(child);
        }

        private static void RestoreOriginalTexts(Control control)
        {
            if (_originalTexts.TryGetValue(control, out StrongBox<string> box))
            {
                string original = box.Value;
                if (control.Text != original)
                    control.Text = original;
            }

            foreach (Control child in control.Controls)
                RestoreOriginalTexts(child);
        }
    }
}
