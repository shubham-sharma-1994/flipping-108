using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{

    public int columns = 4;
    public int rows    = 3;
    public float spacing = 1.3f;
    public GameObject cardPrefab;
    public Sprite cardBackSprite;
    public Sprite[] cardFrontSprites;

    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameOverSound;

    private AudioSource audioSource;

    private List<Card> allCards = new List<Card>();
    private Card pendingFirst;
    private bool canSelect = true;    

    public int Matches   { get; private set; }
    public int Turns     { get; private set; }
    public int TotalPairs { get; private set; }

    public UnityEvent<int, int> OnScoreChanged;  
    public UnityEvent OnGameWon;

    private List<int> cardIdLayout = new List<int>();

    void Awake()
    {
        if (OnScoreChanged == null) OnScoreChanged = new UnityEvent<int, int>();
        if (OnGameWon == null)      OnGameWon      = new UnityEvent();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    public void CleanUpBoard()
    {
        StopAllCoroutines();
        foreach (var c in allCards)
            if (c != null) Destroy(c.gameObject);
        allCards.Clear();
        pendingFirst  = null;
        canSelect = true;
    }

    public void InitGame()
    {
        foreach (var c in allCards)
            if (c != null) Destroy(c.gameObject);
        allCards.Clear();
        pendingFirst  = null;
        canSelect = true;
        Matches = 0;
        Turns   = 0;

        int total = columns * rows;
        if (total % 2 != 0)
        {
            Debug.LogError("Board size must be even! Adjusting columns +1.");
            columns++;
            total = columns * rows;
        }
        TotalPairs = total / 2;

        if (cardBackSprite == null)
        {
            Debug.LogError("GameManager: cardBackSprite is not assigned! Drag a card-back sprite into the Inspector.");
            return;
        }
        if (cardFrontSprites == null || cardFrontSprites.Length < TotalPairs)
        {
            Debug.LogError($"GameManager: Need at least {TotalPairs} front sprites but only {(cardFrontSprites == null ? 0 : cardFrontSprites.Length)} assigned!");
            return;
        }

        List<int> ids = new List<int>();
        for (int i = 0; i < TotalPairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }
        ShuffleList(ids);

        if (cardPrefab == null)
        {
            Debug.LogError("GameManager: cardPrefab is not assigned! Drag the Card prefab into the Inspector.");
            return;
        }

        Vector3 startPos = new Vector3(
            -(columns - 1) * spacing * 0.5f,
             (rows    - 1) * spacing * 0.5f,
             0f);

        for (int i = 0; i < total; i++)
        {
            int col = i % columns;
            int row = i / columns;
            Vector3 pos = startPos + new Vector3(col * spacing, -row * spacing, 0f);

            GameObject go = Instantiate(cardPrefab, pos, Quaternion.identity, transform);
            go.SetActive(true);   
            go.name = $"Card_{ids[i]}_{i}";

            Card card = go.GetComponent<Card>();
            if (card == null) card = go.AddComponent<Card>();

            int id = ids[i];
            card.Init(id, cardFrontSprites[id], Color.white, cardBackSprite, this);
            allCards.Add(card);
        }

        OnScoreChanged?.Invoke(Matches, Turns);

        SaveGame();

        StartCoroutine(PreviewAllCards(3f));
    }

    public void SaveGame()
    {
        var data = new GameSaveData
        {
            columns    = columns,
            rows       = rows,
            matches    = Matches,
            turns      = Turns,
            totalPairs = TotalPairs
        };

        foreach (var card in allCards)
        {
            data.cards.Add(new CardSaveData
            {
                cardId    = card.CardId,
                isMatched = card.IsMatched
            });
        }

        SaveManager.Save(data);
    }

    public void ClearSave()
    {
        SaveManager.DeleteSave();
    }

    public bool LoadGame()
    {
        GameSaveData data = SaveManager.Load();
        if (data == null || !data.IsInProgress)
            return false;

        foreach (var c in allCards)
            if (c != null) Destroy(c.gameObject);
        allCards.Clear();
        pendingFirst = null;
        canSelect    = true;

        columns    = data.columns;
        rows       = data.rows;
        Matches    = data.matches;
        Turns      = data.turns;
        TotalPairs = data.totalPairs;

        if (cardBackSprite == null || cardFrontSprites == null || cardFrontSprites.Length < TotalPairs)
        {
            Debug.LogError("GameManager.LoadGame: sprites not assigned or insufficient for saved board.");
            return false;
        }
        if (cardPrefab == null)
        {
            Debug.LogError("GameManager.LoadGame: cardPrefab not assigned.");
            return false;
        }

        int total = columns * rows;
        if (data.cards.Count != total)
        {
            Debug.LogError("GameManager.LoadGame: card count mismatch in save data.");
            return false;
        }

        Vector3 startPos = new Vector3(
            -(columns - 1) * spacing * 0.5f,
             (rows    - 1) * spacing * 0.5f,
             0f);

        for (int i = 0; i < total; i++)
        {
            int col = i % columns;
            int row = i / columns;
            Vector3 pos = startPos + new Vector3(col * spacing, -row * spacing, 0f);

            CardSaveData cs = data.cards[i];
            int id = cs.cardId;

            GameObject go = Instantiate(cardPrefab, pos, Quaternion.identity, transform);
            go.SetActive(true);
            go.name = $"Card_{id}_{i}";

            Card card = go.GetComponent<Card>();
            if (card == null) card = go.AddComponent<Card>();

            card.Init(id, cardFrontSprites[id], Color.white, cardBackSprite, this);

            if (cs.isMatched)
                card.RestoreMatched();

            allCards.Add(card);
        }

        OnScoreChanged?.Invoke(Matches, Turns);
        return true;
    }

    private IEnumerator PreviewAllCards(float duration)
    {
        canSelect = false;

        foreach (var card in allCards)
            card.Reveal();

        yield return new WaitForSeconds(duration);

        foreach (var card in allCards)
            card.Hide();

        yield return new WaitForSeconds(0.4f);
        canSelect = true;
    }

    public void CardClicked(Card card)
    {
        if (!canSelect || card.IsRevealed || card.IsMatched || card.IsAnimating)
            return;

        card.Reveal();
        PlaySound(flipSound);

        if (pendingFirst == null)
        {
            pendingFirst = card;
        }
        else
        {
            Card first  = pendingFirst;
            Card second = card;
            pendingFirst = null;

            Turns++;
            OnScoreChanged?.Invoke(Matches, Turns);
            StartCoroutine(CheckMatch(first, second));
        }
    }

    private IEnumerator CheckMatch(Card first, Card second)
    {
        yield return new WaitForSeconds(0.8f);

        if (first == null || second == null) yield break;

        if (first.CardId == second.CardId)
        {
            first.MatchFound();
            second.MatchFound();
            Matches++;
            PlaySound(matchSound);
            OnScoreChanged?.Invoke(Matches, Turns);

            if (Matches >= TotalPairs)
            {
                Debug.Log($"You Win!  Turns: {Turns}");
                PlaySound(gameOverSound);
                ClearSave();
                OnGameWon?.Invoke();
            }
            else
            {
                SaveGame();
            }
        }
        else
        {
            PlaySound(mismatchSound);
            first.Hide();
            second.Hide();
            SaveGame();
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp   = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}
