using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class UIManager : MonoBehaviour
{
    [Header("UI列表"), ReadOnly, SerializeField]
    public List<UIBase> uiNameObjPairs = new List<UIBase>();

    private static Dictionary<string, UIBase> uiElements = new Dictionary<string, UIBase>();

    [Header("UI动画参数")]
    public float fadeOutDuration = 0.7f;
    public float fadeInDuration = 0.3f;

    //是否正在发生UI切换
    private bool isTransitioning = false;
    //正在执行的UI切换协程
    private Coroutine currentTransitionCoroutine = null;
    //切换完毕后执行回调
    private UnityAction switchFinishCallback = null;

    private UIBase currentUI = null;

    private void Awake()
    {
        TryInitUIElement();
        DontDestroyOnLoad(gameObject);
    }

    private bool TryInitUIElement()
    {
        uiElements.Clear();

        foreach (UIBase pair in uiNameObjPairs)
        {
            if (uiElements.ContainsKey(pair.GetName()))
            {
                Debug.LogError(string.Format("噢不！有UI重名了！重名UI名称：{0}", pair.GetName()), pair.gameObject);
                return false;
            }
            uiElements.Add(pair.GetName(), pair);
        }
        return true;
    }

    public void SwitchUI(string newUIName, UnityAction finishCallback = null)
    {
        if (isTransitioning && currentTransitionCoroutine != null)
        {
            StopCoroutine(currentTransitionCoroutine);
        }

        switchFinishCallback = finishCallback;

        if (uiElements.ContainsKey(newUIName))
        {
            UIBase newUI = uiElements[newUIName];
            if (currentUI)
            {
                currentTransitionCoroutine = StartCoroutine(SwitchAfterFadeOut(currentUI, newUI));
            }
            else
            {
                currentTransitionCoroutine = StartCoroutine(DirectFadeIn(newUI));
            }
            currentUI = newUI;
        }
    }


    //简单的淡入淡出动画
    IEnumerator DirectFadeIn(UIBase ui)
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeInUI(ui));
        isTransitioning = false;
        switchFinishCallback?.Invoke();
        switchFinishCallback = null;
    }

    IEnumerator SwitchAfterFadeOut(UIBase oldUI, UIBase newUI)
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeOutUI(oldUI));
        yield return StartCoroutine(FadeInUI(newUI));
        isTransitioning = false;
        switchFinishCallback?.Invoke();
        switchFinishCallback = null;
    }

    IEnumerator FadeInUI(UIBase ui)
    {
        ui.Show();
        CanvasGroup canvasGroup = ui.GetComponent<CanvasGroup>();
        if (!canvasGroup)
        {
            canvasGroup = ui.canvas.gameObject.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, (elapsedTime / fadeInDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    //动画效果实现
    IEnumerator FadeOutUI(UIBase ui)
    {
        CanvasGroup canvasGroup = ui.GetComponent<CanvasGroup>();
        if (!canvasGroup)
        {
            canvasGroup = ui.canvas.gameObject.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0;
        while (elapsedTime < fadeOutDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, (elapsedTime / fadeOutDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        ui.Hide();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIManager))]
    public class UIManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("更新UI节点"))
            {
                var script = target as UIManager;
                script.uiNameObjPairs.Clear();
                var uiroot = script.transform.Find("UIs");
                if (uiroot)
                {
                    foreach (Transform child in uiroot)
                    {
                        var ui = child.GetComponent<UIBase>();
                        if (ui)
                        {
                            script.uiNameObjPairs.Add(ui);
                        }
                        else
                        {
                            Debug.LogError(string.Format("{0} 没有挂逻辑脚本", ui.gameObject.name), ui.gameObject);
                        }
                    }
                }
                else
                {
                    Debug.LogError("请检查[UIManager/UIs]节点是否存在");
                }
                if (!script.TryInitUIElement())
                {
                    EditorUtility.DisplayDialog("警告", "请检查Console窗口的错误信息并修复", "我知道了");
                }
            }
        }
    }
#endif
}
