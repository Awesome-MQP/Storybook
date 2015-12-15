using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

    // Screen to open automatically at the start
    public Animator initiallyOpen;
    // Currently open screen
    private Animator m_Open;
    // ID of the parameter we use to control the transitions
    private int m_OpenParameterID;

    // The GameObject selected before we opened the current Screen
    // Used when closing a screen, so we can go back to the button that opened it.
    private GameObject m_PreviouslySelected;

    // Animator State and Transition names we need to check against
    const string OpenTransitionName = "Open";
    const string ClosedStateName = "Closed";

    private static readonly int m_MAX_NUM_UIs = 16;

    private Canvas[] m_UIScreens = new Canvas[m_MAX_NUM_UIs];

	// Load in all the UIs from the UIPrefabs folder
	void Awake () {
        DontDestroyOnLoad(this);
        //m_UIScreens = Resources.LoadAll("UIPrefabs", GameObject) as Canvas[];
        m_UIScreens = Resources.LoadAll<Canvas>("UIPrefabs");
	}
    
    public void OnEnable()
    {
        // Cache the ID to "open" param, so can feed to Animator.SetBool
        m_OpenParameterID = Animator.StringToHash(OpenTransitionName);

        //if set, open the inital screen now
        if(initiallyOpen==null)
        { return; }
        OpenPanel(initiallyOpen);
    }

    // close the currently open panel and opens the provided one
    public void OpenPanel(Animator anim)
    {
        if (m_Open == anim)
        { return; }

        // Activate the new screen hierarcy so we can animate it
        anim.gameObject.SetActive(true);
        // Save the currently selected button that was used to open this screen.
        GameObject newPreviouslySelected = EventSystem.current.currentSelectedGameObject;
        // Move the screen to the front
        anim.transform.SetAsLastSibling();

        CloseCurrent();

        m_PreviouslySelected = newPreviouslySelected;

        // Set the new screen as then open one
        m_Open = anim;
        // start open animation
        m_Open.SetBool(m_OpenParameterID, true);

        // set an element in the new screen as the new selected one
        GameObject go = FindFirstEnabledSelectable(anim.gameObject);
        SetSelected(go);
    }

    // Finds the first selectable element in the provided hierarchy
    static GameObject FindFirstEnabledSelectable(GameObject gameObject)
    {
        GameObject go = null;
        Selectable[] selectables = gameObject.GetComponentsInChildren<Selectable>(true);
        foreach (var selectable in selectables)
        {
            if(selectable.IsActive() && selectable.IsInteractable())
            {
                go = selectable.gameObject;
                break;
            }
        }
        return go;
    }

    // close the currently open screen
    // also takes care of navigation
    // reverting selection to the Selectable used before opening the current screen
    public void CloseCurrent()
    {
        if(m_Open == null) { return; }

        // start the close animation
        m_Open.SetBool(m_OpenParameterID, false);

        // reverting selection to the Selectable used before opening the current screen
        SetSelected(m_PreviouslySelected);
        // start coroutine to disable the hierarchy when the closing animation finishes
        this.StartCoroutine(DisablePanelDeleyed(m_Open));
        // no screen open
        m_Open = null;
    }

    // coroutine that will detect when the closing animation is finished and it will deactivate the hierarchy
    IEnumerator DisablePanelDeleyed(Animator anim)
    {
        bool closedStateReached = false;
        bool wantToClose = true;
        while(!closedStateReached && wantToClose)
        {
            if(!anim.IsInTransition(0))
            {
                closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(ClosedStateName);
            }
            wantToClose = !anim.GetBool(m_OpenParameterID);
            yield return new WaitForEndOfFrame();
        }
        if(wantToClose) { anim.gameObject.SetActive(false); }
    }

    // make the provided GameObject selected
    // when using mouse/touch we actually want to set it as the previously selected and
    // set nothing as the selected for now
    private void SetSelected(GameObject go)
    {
        // select the GO
        EventSystem.current.SetSelectedGameObject(go);

        // if we are using the keyboard right now, that's all we need to do
        StandaloneInputModule standaloneInputModule = EventSystem.current.currentInputModule as StandaloneInputModule;
        if(standaloneInputModule != null) //  && standaloneInputModule.inputMode == StandaloneInputModule.InputMode.Buttons)
        { return; }

        // since we are using a pointer device, we don't want anything selected
        // but if the user switches to the keyboard, we want to start navigation from the provided game object
        // so here we set the current Selected to null, so the provided GO becomes the last selected int he EventSystem
        EventSystem.current.SetSelectedGameObject(null);
    }
    
}
