using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] TMP_Text promptDisplay;

    [SerializeField] float characterDisplaySpeed = 0.05f;
    [SerializeField] float timeBetweenPrompts = 3.0f;

    [SerializeField] PlayerController tutorialPlayer;

    [SerializeField] GameObject jumpPlatform;
    [SerializeField] GameObject climbingWall;


    public UnityEvent conditionMet = new UnityEvent();
    bool promptOver = false;
    bool allPromptsOver = false;

   public int conditionTracker = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        StartCoroutine( PlayerWelcome());
        jumpPlatform.SetActive(false);
        climbingWall.SetActive(false);
    }

    public void DisablePlayer()
    {
        foreach (var action in tutorialPlayer._playerInput.actions)
        {
            action.Disable();
        }
    }

    IEnumerator PlayerWelcome()
    {
        DisablePlayer();   

        string[] prompts =
        {
            "Welcome to the tutorial!",
            "You have begun your path to become a true tetherball warrior!",
            "Let's begin by going over basic movement. It's important to understand the basics,",
            "before we get to the more advanced parts.",
        };
        yield return StartCoroutine(DisplayPrompts(prompts));
        yield return StartCoroutine(WalkMovement());
    }

    IEnumerator WalkMovement()
    {
        tutorialPlayer._playerInput.actions["Move"].Enable();
        DisplayPrompt("Start with basic movement back and forth.");
        conditionTracker = 0;

        yield return new WaitUntil(() => conditionTracker >= 2);
        DisablePlayer();
        string[] prompts =
        {
            "Excellent. Though basic, movement is the key to staying alive-",
            "as well as landing strikes on your opponents.",
            "Lets continue with jumping."
        };
       yield return StartCoroutine (DisplayPrompts(prompts));
        yield return new WaitUntil( () => allPromptsOver);
        StartCoroutine(JumpMovement());
    }

    IEnumerator JumpMovement()
    {

        string[] prompts =
        {
            "You can press the " + tutorialPlayer._playerInput.actions["Jump"].controls[0].name + " button to jump.",
        };

        StartCoroutine(DisplayPrompts(prompts));
        yield return new WaitUntil(() => allPromptsOver);
        jumpPlatform.SetActive(true);
        prompts[0] = "See that platform over there? Try to jump on top of it.";
        StartCoroutine(DisplayPrompts(prompts));
        yield return new WaitUntil(() => allPromptsOver);


        tutorialPlayer._playerInput.actions["Move"].Enable();
        tutorialPlayer._playerInput.actions["Jump"].Enable();
        EnableActionsExclusive("Move", "Jump");
        conditionTracker = 0;
        yield return new WaitUntil(() => conditionTracker >= 1);
        DisablePlayer();

        string[] newPrompts = {
            "Good work. Jumping is the most important way to dodge incoming attacks,",
             "as well as allowing future options that we'll go over later.",
             "Almost done with the basics now."
        };

        StartCoroutine(DisplayPrompts(newPrompts));
        yield return new WaitUntil(() => allPromptsOver);

        StartCoroutine(ClimbMovement());
    }

    IEnumerator ClimbMovement()
    {
        DisablePlayer();
        string[] prompts =
        {
            "As a nimble, and much younger knight than I, you have the ability to climb walls.",
            "This lets you quickly get to areas you can't reach just by jumping.",
        };
        yield return StartCoroutine(DisplayPrompts(prompts));
        
        climbingWall.SetActive(true);

        string[] promptsTwo =
        {
            "To start a climb, jump up and hold the direction of the wall.",
            "The wall's on your right, so make sure to hold that input when you touch it.",
            "At any time, you can cancel a climb into a jump, or hold the opposite direction-",
            "to disconnect from it.",
            "Hold up to climb up, or hold down to slide."
        };

        yield return StartCoroutine (DisplayPrompts(promptsTwo));
        conditionTracker = 0;
        tutorialPlayer._playerInput.actions["Move"].Enable();
        tutorialPlayer._playerInput.actions["Jump"].Enable();

        yield return new WaitUntil(() => conditionTracker >= 2);

        string[] promptsThree =
        {
            "Your ability to scale walls like this makes you quite the meanace!",
            "Now, let's put to the test what've you've learned."
        };
        yield return StartCoroutine(DisplayPrompts(promptsThree));
        Debug.Log("End of basic movement");

        StartCoroutine(Grapple());
    }

    public IEnumerator Grapple()
    {
        yield return null;
    }


    public void DisplayPrompt(string prompt)
    {
        promptOver = false;
        promptDisplay.text = string.Empty;
        StartCoroutine(DisplayNextCharacter(prompt));

    }

    public IEnumerator DisplayPrompts(string[] prompts)
    {
        allPromptsOver = false;
        for (int i = 0; i < prompts.Length; i++)
        {
            promptOver = false;
            promptDisplay.text = string.Empty;
            StartCoroutine (DisplayNextCharacter(prompts[i]));
            yield return new WaitUntil( () => promptOver);
            yield return new WaitForSeconds(timeBetweenPrompts);
        }
        allPromptsOver = true;
    }

    IEnumerator DisplayNextCharacter(string prompt)
    {
        for (int i = 0; i < prompt.Length; i++)
        {
            promptDisplay.text += prompt[i];
            yield return new WaitForSeconds(characterDisplaySpeed);
        }
        promptOver = true;
    }

    public void EnableActionsExclusive(params string[] actions)
    {
        DisablePlayer();
        foreach (string action in actions)
        {
            tutorialPlayer._playerInput.actions[action].Enable();
        }
        //exclusive means we turn off every action except for the requested ones
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}