using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.UI;
using Il2CppSystem;
using Il2CppTMPro;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace QuickReload 
{
    public class Core : MelonMod
    {
        public Core Instance;
        private GameObject pauseMenuObject;
        private GameObject saveAndReloadButton;
        private bool gameLoaded = false;
        public override void OnInitializeMelon()
        {
            Instance = this;
            LoggerInstance.Msg("✅Quick Reload Initialized.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            try
            {
                if (sceneName.ToUpper() == "MAIN")
                {
                    gameLoaded = true;
                    MelonCoroutines.Start(SaveAndReloadButtonCoroutine());
                }
                else
                {
                    gameLoaded = false;
                    if (saveAndReloadButton != null)
                    {
                        UnityEngine.Object.Destroy(saveAndReloadButton);
                        saveAndReloadButton = null;
                    }
                    pauseMenuObject = null;
                }
            }
            catch (System.Exception ex)
            {
                LoggerInstance.Error("Error in public override void OnSceneWasLoaded(): " + ex);
            }
        }

        private IEnumerator SaveAndReloadButtonCoroutine()
        {
            yield return (object)new WaitForSeconds(2f);
            InitializePauseMenuObject();
            CreateSaveAndReloadButton();
        }

        private void InitializePauseMenuObject()
        {
            try
            {
                PauseMenu pauseMenu = ((Il2CppObjectBase)UnityEngine.Object.FindObjectOfType(Il2CppType.Of<PauseMenu>()))?.TryCast<PauseMenu>();
                if (pauseMenu != null)
                {
                    pauseMenuObject = pauseMenu.gameObject;
                    if (pauseMenu.Container == null)
                    {
                        LoggerInstance.Error("Could not get pause menu container.");
                        return;
                    }
                }
                else
                {
                    LoggerInstance.Error("Could not get pause menu instance");
                }
            }
            catch (System.Exception ex)
            {
                LoggerInstance.Error("Error in private void InitializePauseMenuObject(): " + ex);
            }
            
        }

        private void CreateSaveAndReloadButton()
        {
            try
            {
                if (!gameLoaded)
                    return;

                if (pauseMenuObject == null)
                {
                    LoggerInstance.Error("pauseMenuObject is null.");
                    return;
                }

                PauseMenu pauseMenu = pauseMenuObject.GetComponent<PauseMenu>();
                if (pauseMenu == null)
                {
                    LoggerInstance.Error("pauseMenu is null.");
                    return;
                }

                RectTransform pauseMenuTransform = pauseMenu.Container;
                if (pauseMenuTransform == null)
                {
                    LoggerInstance.Error("pauseMenuTransform is null.");
                    return;
                }

                Transform bankTransform = pauseMenuTransform.Find("Container/Bank");
                if (bankTransform == null)
                {
                    LoggerInstance.Error("bankTransform is null.");
                    return;
                }

                if (saveAndReloadButton != null)
                    return;

                Transform quitTransfrom = bankTransform.Find("Quit");
                if (quitTransfrom == null)
                {
                    LoggerInstance.Error("quitTransform is null.");
                    return;
                }

                saveAndReloadButton = UnityEngine.Object.Instantiate<GameObject>(quitTransfrom.gameObject, bankTransform);
                if (saveAndReloadButton == null)
                {
                    LoggerInstance.Error("saveAndReloadButton did not instantiate.");
                    return;
                }

                saveAndReloadButton.name = "SaveAndReloadButton";
                TextMeshProUGUI textMeshPro = saveAndReloadButton.GetComponentInChildren<TextMeshProUGUI>();
                if (textMeshPro == null)
                {
                    LoggerInstance.Error("Failed to get textMeshPro in saveAndReloadButton.");
                    return;
                }

                textMeshPro.text = "Save & Reload";
                Button button = saveAndReloadButton.GetComponent<Button>();
                if (button == null)
                {
                    LoggerInstance.Error("Failed to get button in saveAndReloadButton.");
                    return;
                }

                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener((UnityEngine.Events.UnityAction)(() => SaveAndReload()));

                RectTransform saveAndReloadButtonTransfrom = saveAndReloadButton.GetComponent<RectTransform>();
                if (saveAndReloadButtonTransfrom == null)
                {
                    LoggerInstance.Error("Failed to get saveAndReloadButtonTransfrom in saveAndReloadButton.");
                    return;
                }

                RectTransform QuitButtonTransfrom = quitTransfrom.GetComponent<RectTransform>();
                if (QuitButtonTransfrom == null)
                {
                    LoggerInstance.Error("Failed to get QuitButtonTransfrom in quitTransfrom.");
                    return;
                }

                saveAndReloadButtonTransfrom.anchoredPosition = new Vector2(QuitButtonTransfrom.anchoredPosition.x, QuitButtonTransfrom.anchoredPosition.y - 40.0f);
                saveAndReloadButton.SetActive(true);
            }
            catch (System.Exception ex)
            {
                LoggerInstance.Error("Error in private void CreateSaveAndReloadButton():" + ex);
            }
            
        }

        private void SaveAndReload()
        {
            Singleton<SaveManager>.Instance.Save();
            SaveInfo activeSaveInfo = Singleton<LoadManager>.Instance.ActiveSaveInfo;
            Task.Delay(1000);
            Singleton<LoadManager>.Instance.ExitToMenu(null, null, false);
            Task.Delay(2000);
            Singleton<LoadManager>.Instance.StartGame(activeSaveInfo, false);
        }
    }
}
