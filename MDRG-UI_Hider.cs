using System;
using System.Collections.Generic;
using System.Reflection;
using Il2CppInterop.Runtime;
using MelonLoader;
using UnityEngine;


[assembly: MelonInfo(typeof(HideInterfaceMod.MainHideUIClass), "UI_Hider", "1.0.0", "Paul.Epifancev #warlockforbugs (Discord: anon.jun)")]
[assembly: MelonGame("IncontinentCell", "My Dystopian Robot Girlfriend")]

namespace HideInterfaceMod
{
    public class MainHideUIClass : MelonMod
    {
        private bool isUiHidden = false;
        private float _lastPTime = 0f;
        private const float KeyDebounceTime = 0.4f;
        private bool _pWasPressed = false;

        // Низкоуровневый кэш метода ввода Unity через чистую рефлексию
        private static MethodInfo _getKeyMethod = typeof(Input).GetMethod("GetKey", new Type[] { typeof(KeyCode) });

        // Списки кэширования холстов
        private readonly List<Canvas> _cachedCanvases = new List<Canvas>();

        // Белый список: Имена Canvas, которые мы ОБЯЗАТЕЛЬНО должны скрыть
        private readonly List<string> _canvasWhiteList = new List<string> { 
            "canvas", "hud", "interface", "ui", "overlay", "popup", "dialogue" 
        };

        // Чёрный список: Критические холсты, которые НЕЛЬЗЯ трогать (например, системные или задники)
        private readonly List<string> _canvasBlackList = new List<string> { 
            "dontdestroy", "rendersurfaces", "background_canvas" 
        };

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("==================================================");
            LoggerInstance.Msg("===    UI_HIDER MONOLITHIC CANVAS BLINDNESS    ===");
            LoggerInstance.Msg("==================================================");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            this.isUiHidden = false;
            this._cachedCanvases.Clear();
        }
        public override void OnUpdate()
        {
            float currentTime = Time.time;
            bool pCurrentlyPressed = false;

            try
            {
                
                if (_getKeyMethod != null)
                {
                    pCurrentlyPressed = (bool)_getKeyMethod.Invoke(null, new object[] { 45 });
                }
            }
            catch { }

            if (pCurrentlyPressed && !this._pWasPressed)
            {
                if (currentTime - this._lastPTime >= KeyDebounceTime)
                {
                    this._lastPTime = currentTime;
                    this.isUiHidden = !this.isUiHidden;
                    
                    LoggerInstance.Msg($"[UI Hider] Global Canvas Suppression State: Hidden = {this.isUiHidden}");
                    
                    // Переключаем всю отфильтрованную матрицу холстов строго в один такт (Защита FPS)
                    ToggleAllFilteredCanvases(!this.isUiHidden);
                }
            }
            this._pWasPressed = pCurrentlyPressed;
        }private void RefreshCanvasCache()
        {
            this._cachedCanvases.Clear();
            
            // Находим абсолютно все Canvas в оперативной памяти текущей сессии игры
            var allCanvases = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Canvas>());
            
            for (int i = 0; i < allCanvases.Length; i++)
            {
                Canvas canvas = allCanvases[i].TryCast<Canvas>();
                if (canvas == null || canvas.gameObject == null) continue;

                string canvasName = canvas.gameObject.name.ToLower();
                bool isTarget = false;

                // 1. Проверяем белый список совпадений
                foreach (var whiteItem in _canvasWhiteList)
                {
                    if (canvasName.Contains(whiteItem))
                    {
                        isTarget = true;
                        break;
                    }
                }

                // 2. Жестко отсекаем по чёрному списку исключений
                foreach (var blackItem in _canvasBlackList)
                {
                    if (canvasName.Contains(blackItem))
                    {
                        isTarget = false;
                        break;
                    }
                }

                // Если Canvas прошёл интеллектуальный отбор — добавляем в рабочий стек
                if (isTarget && !this._cachedCanvases.Contains(canvas))
                {
                    this._cachedCanvases.Add(canvas);
                }
            }
        }

        private void ToggleAllFilteredCanvases(bool state)
        {
            try
            {
                // Запускаем динамический сбор и фильтрацию прямо в момент клика
                RefreshCanvasCache();

                int affectedCount = 0;
                for (int i = 0; i < this._cachedCanvases.Count; i++)
                {
                    Canvas canvas = this._cachedCanvases[i];
                    if (canvas != null && canvas.gameObject != null)
                    {
                        canvas.enabled = state;
                        affectedCount++;
                    }
                }
                
                LoggerInstance.Msg($"[Hardware Reset] Pipeline state flipped to: {state}. Canvases processed: {affectedCount}");
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"[UI Hider Fail] Global loop collapsed: {ex.Message}");
            }
        }
    }}
