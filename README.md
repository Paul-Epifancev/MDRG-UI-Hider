# MDRG UI Hider

A lightweight open-source MelonLoader mod for **My Dystopian Robot Girlfriend** that allows players to instantly toggle the visibility of the game's user interface for clean screenshots.

*Лёгкий open-source мод на базе MelonLoader для My Dystopian Robot Girlfriend, позволяющий мгновенно переключать видимость интерфейса игры для создания чистых скриншотов.*

---

## ⚙️ Features / Возможности

* **Instant Suppression:** Toggles canvas visibility with a single keypress.
* **Smart Filtering:** Uses built-in white/blacklists to target dialogue, popups, and HUD elements while preserving critical background layers.
* **Low-Level Execution:** Utilizes unmanaged Unity Input reflection to bypass standard runtime input locks.
* **Performance Optimized:** Canvas caching and state swapping execute strictly in a single frame to ensure zero FPS drops.

---

## ⌨️ Controls / Управление

* **Minus key (`-`)** on the main keyboard to show/hide the UI.
* *Клавиша **Минус (`-`)** на основной клавиатуре для скрытия/показа интерфейса.*

---

## 🚀 Installation / Установка

1. Make sure you have **MelonLoader** (v0.7.3 or compatible) installed.
2. Download the latest `UI_Hider.dll` from the Releases section.
3. Place the `.dll` file into your game's `Mods/` directory.
4. Launch the game.

---

## 🛠️ Technical Details / Техническая информация

* **Compatibility:** PC (Unity 6 / IL2CPP runtime).
* **Execution:** Dynamically scans volatile VRAM every time the interface pipeline state is flipped using `Resources.FindObjectsOfTypeAll`.
* **Debounce Time:** Enforced `0.4s` safety lock to prevent input stutter.

---

## 👤 Author / Автор

* Created by **Paul.Epifancev** (#warlockforbugs)
* **Discord:** anon.jun

---

## 📄 License / Лицензия

This project is released under the **LICENSE**.
