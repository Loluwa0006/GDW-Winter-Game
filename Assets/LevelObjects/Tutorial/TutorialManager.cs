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
    [SerializeField] Animator animator;


    public UnityEvent conditionMet = new UnityEvent();
    bool promptOver = false;
    bool allPromptsOver = false;

   public int conditionTracker = 0;


    //Basic Movement
    [SerializeField] GameObject MovementTeaching;
    [SerializeField] GameObject MovementTest;
    [SerializeField] GameObject slimePrefab;
    [SerializeField] Transform slimeSpawner;

    


    private void Start()
    {
        conditionTracker = 0;
        StartCoroutine( PlayerWelcome());

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
        ResetPlayerPosition();
        
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
        EnableActionsExclusive("Move");
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
        OnMechanicLearned();
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
        prompts[0] = "See that platform over there? Try to jump on top of it.";
        StartCoroutine(DisplayPrompts(prompts));
        yield return new WaitUntil(() => allPromptsOver);

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
        OnMechanicLearned();
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
        
        string[] promptsTwo =
        {
            "To start climbing, jump up and move into the wall. In this case, hold right.",
            "At any time, you can cancel a climb into a jump, or hold the other way to disconnect.",
            "Hold up to climb, hold down to slide."
        };

        yield return StartCoroutine (DisplayPrompts(promptsTwo));
        conditionTracker = 0;
        tutorialPlayer._playerInput.actions["Move"].Enable();
        tutorialPlayer._playerInput.actions["Jump"].Enable();

        yield return new WaitUntil(() => conditionTracker >= 2);

        string[] promptsThree =
        {
            "Your ability to scale walls like this makes you quite the menace!",
            "Now, let's put to the test what've you've learned."
        };
        yield return StartCoroutine(DisplayPrompts(promptsThree));
        animator.SetBool("MovementLearned", true);
        OnMechanicLearned();
        StartCoroutine(StartMovementTest());
    }
    public IEnumerator StartMovementTest()
    {
      
        animator.SetBool("MovementLearned", true);
        string[] prompts =
        {
            "Try to make it to the destination on the right.",
            "It's ok if you fall, I had the wizards construct a special field to catch you.",
            "Feel free to take as much time as you need."
        };
        DisablePlayer();
        yield return DisplayPrompts(prompts);
        EnableActionsExclusive("Move", "Jump");
        conditionTracker = 0;
        ResetPlayerPosition();
        yield return new WaitUntil(() => conditionTracker >= 1);
        string[] celebration =
        {
            "Good work knight! I wasn't expecting you to get it so soon!",
            "Took me hours last night... my back still aches. Miss being young.",
            "Anyways, you've proven yourself ready to start the next part of your training!"
        };
        DisablePlayer();
        yield return StartCoroutine(DisplayPrompts(celebration));
        StartCoroutine(Tackling());
        animator.ResetTrigger("LearnedMechanic");
        animator.SetBool("FinishedMovement", true);
    }

    public IEnumerator Tackling()
    {
        DisablePlayer();
        
        string[] prompts =
        {
            "Now that we've gone over movement that you'll need to dodge attacks",
            "let's learn how to attack ourselves, shall we?",
            "While you can't throw a punch or kick to save your life,",
            "your speed could be used as an attack.",
            "Press the " + tutorialPlayer._playerInput.actions["Attack"].controls[0].displayName + " button to dash forward, tackling opponents.",
        };
        yield return StartCoroutine(DisplayPrompts(prompts));
        string[] slime =
        {
            "Tackle that slime over there!"
        };
        yield return StartCoroutine (DisplayPrompts(slime));
        EnableActionsExclusive("Attack");
        conditionTracker = 0;
        tutorialPlayer.transform.localScale = new Vector3(1, 1, 1);
        //makes it so they tackle to the right
        yield return new WaitUntil(() => conditionTracker >= 1);
        DisablePlayer();
        string[] reaction =
        {
            "WAIT, NOT THAT HAR-",
            "*crash*",
            "My neck's going to roll for this..."
        };
        yield return StartCoroutine(DisplayPrompts(reaction));
        OnMechanicLearned();
        StartCoroutine(GroundPound());

    }

    public IEnumerator GroundPound()
    {
        ResetPlayerPosition();
        string[] prompts =
        {
            "Well, I'm not even sure if I want to teach you this next technique...",
            "Let's say, hypothetically of course, that if you were to jump,",
            "and then press the attack key while holding a down input,",
            "AND there was another slime who's been saying rather unplesant things about your mother..."
        };

        GameObject newSlime = Instantiate(slimePrefab);
        newSlime.transform.position = slimeSpawner.transform.position;
        conditionTracker = 0;
        yield return StartCoroutine(DisplayPrompts(prompts));

        string[] reaction;
        newSlime.transform.position = slimeSpawner.transform.position;
        newSlime.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        EnableActionsExclusive("Jump", "Attack", "Move");
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
        promptDisplay.color = Color.white;
        allPromptsOver = false;
        for (int i = 0; i < prompts.Length; i++)
        {
            promptOver = false;
            promptDisplay.text = string.Empty;
            StartCoroutine (DisplayNextCharacter(prompts[i]));
            yield return new WaitUntil( () => promptOver);
            yield return new WaitForSeconds(timeBetweenPrompts);
        }
        promptDisplay.color = Color.green;
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

   void OnMechanicLearned()
    {
        animator.SetTrigger("LearnedMechanic");
    }

    public void ResetPlayerPosition()
    {
        tutorialPlayer.transform.position = tutorialPlayer._respawnPoint.transform.position;
        tutorialPlayer.transform.localScale = tutorialPlayer._respawnPoint.transform.localScale;
    }
}