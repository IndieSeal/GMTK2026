using System.Collections;
using Febucci.UI.Core;
using UnityEngine;

public class Tutorial1 : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TypewriterCore typewriter;
    [SerializeField] private Animator animator;
    private Candle candle;
    
    [Header("Phase 1")]
    [SerializeField] private HitReceiver phase1Collider;

    [Space]
    [SerializeField] private string dialogue1 = "Hey, welcome to the mansion! I haven't seen a human here in a while.";
    [SerializeField] private string dialogue2 = "Looks like the monster won't let you out, let's defeat them ig yuppee";
    [SerializeField] private string dialogue3;
    private bool hasEnteredPhase1;

    void Awake()
    {
        candle = FindAnyObjectByType<Candle>();
    }

    void OnEnable()
    {
        phase1Collider.OnAnyHit += EnterPhase1Area;

        Door.OnRoomChanged += CancelDialogue;
    }

    void OnDisable()
    {
        phase1Collider.OnAnyHit -= EnterPhase1Area;

        Door.OnRoomChanged -= CancelDialogue;
    }

    void Update()
    {
        candle.AddCandleDuration(3);
    }

    private void EnterPhase1Area(GameObject g)
    {
        if(hasEnteredPhase1 || !g.TryGetComponent(out PlayerMovement player)) return;

        hasEnteredPhase1 = true;
        StartCoroutine(EnterPhase1Coroutine());
    }

    private IEnumerator EnterPhase1Coroutine()
    {
        yield return WaitForDialogue(dialogue1);
        yield return WaitForDialogue(dialogue2);
        if(!string.IsNullOrEmpty(dialogue3)) yield return WaitForDialogue(dialogue3);
    }

    private void CancelDialogue()
    {
        StopAllCoroutines();
        typewriter.SkipTypewriter();
        EndDialogue();
    }

    #region General

    private bool waitForDialogue = false;
    private IEnumerator WaitForDialogue(string dialogue, float endDelay = 1)
    {
        waitForDialogue = false;
        
        dialogueParent.SetActive(true);

        animator.SetBool("IsTalking", true);
        
        typewriter.TextAnimator.textFull = $"<?start>{dialogue}";
        typewriter.onTextShowed.AddListener(OnDialogueComplete);

        while(!waitForDialogue) yield return null;

        yield return new WaitForSeconds(endDelay);
        EndDialogue();
    }

    private void EndDialogue()
    {
        animator.SetBool("IsTalking", false);
        
        typewriter.onTextShowed.RemoveListener(OnDialogueComplete);
        
        dialogueParent.SetActive(false);
        waitForDialogue = false;
    }

    private void OnDialogueComplete()
    {
        waitForDialogue = true;
    }

    #endregion
}