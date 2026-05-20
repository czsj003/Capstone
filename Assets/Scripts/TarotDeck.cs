using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotDeck : PlayerWeapon
{
    private const int DECK_SIZE = 56;
    private List<Card> CardDeck = new List<Card>();    // Available cards to pick when redrawing.
    private List<Card> Discard = new List<Card>();     // Thrown cards to be restored on next Shuffle().
    public List<Card> Hand = new List<Card>();
    public int handSize;   // Number of cards that can be drawn at once
    [SerializeField] private Sprite[] sprites;  // The back sprite and all face sprites for the cards
    private AudioSource drawHandSound;

    // Card projectile
    [SerializeField] private GameObject cardPrefab; // Prefab for the card projectile
    [SerializeField] private Sprite noSprite; // Assign this in the Inspector for testing

    private bool isThrowing = false;

    private void Start()
    {
        drawHandSound = GetComponent<AudioSource>();

        if (noSprite == null)
        {
            Debug.LogError("NoSprite could not be loaded. Make sure it is in the Resources folder.");
            return;
        }

        // Initialize sprites array with NoSprite
        sprites = new Sprite[DECK_SIZE]; // Ensure this matches the deck size or desired size
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = noSprite;
        }

        Debug.Log("Sprites array populated with NoSprite for testing.");

        // Now proceed with normal deck initialization using `sprites`
        CardDeck.Capacity = DECK_SIZE;
        Discard.Capacity = DECK_SIZE;

        // UPDATE LATER
        float uniSpeed = 10; int uniBounces = 3; int uniPierce = 3; int uniKB = 3;
        List<int[]> dmg = new List<int[]>();
        for (int i = 0; i < 14; i++)
        {
            int[] a = { i + 1, i + 3 };
            dmg.Add(a);
        }

        // Populate CardDeck with placeholder cards
        for (int i = 0; i < sprites.Length; i++)
        {
            Card newCard = new Card(dmg[i % 14][0], dmg[i % 14][1], uniKB, uniSpeed, uniBounces, uniPierce, sprites[i], sprites[0]);
            CardDeck.Add(newCard);
            Debug.Log("Added placeholder card to deck.");
        }

        Debug.Log("Deck initialized with " + CardDeck.Count + " cards.");

        Debug.Log("Hand size set to: " + handSize);


        // Randomize and draw
        RandomizeDeck();
        Draw();
    }

    void Update()
    {
        if(Player.S.CanMove)
        {
            if (Input.GetMouseButtonDown(0) && Hand.Count > 0) // Left Mouse Button
            {
                PrimaryAttack();
            }

            if (Input.GetMouseButtonDown(1) && Hand.Count > 0) // Right Mouse Button
            {
                SecondaryAttack();
            }
        }

    }

    private void Draw()
    {
        Debug.Log("Drawing new hand");
        if (CardDeck.Count < 1) Shuffle();
        drawHandSound.Play();
        for (int i = 0; i < handSize; i++)
        {
            if (CardDeck.Count < 1) break;
            Hand.Add(CardDeck[0]);
            Debug.Log("Added card to hand: " + CardDeck[0].face.name);
            CardDeck.RemoveAt(0);
        }

        Debug.Log("Hand now contains " + Hand.Count + " cards.");
    }

    public void Shuffle()
    {
        while (Discard.Count > 0)
        {
            CardDeck.Add(Discard[0]);
            Discard.RemoveAt(0);
        }
        RandomizeDeck();
    }

    void RandomizeDeck()
    {
        for (int i = CardDeck.Count - 1; i >= 0; i--)
        {
            int j = (int)Random.Range(0, i + 1);
            Card temp = CardDeck[j];
            CardDeck[j] = CardDeck[i];
            CardDeck[i] = temp;
        }
    }

    public override void PrimaryAttack()
    {
        if (Hand.Count < 1)
        {
            Debug.LogError("Hand is empty");
            return;
        }

        if (!isThrowing)
        {
            // Get the first card and its properties
            Card card = Hand[0];
            int damage = card.GetCardDamage();
            int knockback = card.Knockback;
            float speed = card.Speed;
            int bounces = card.Bounces;
            int pierce = card.Pierce;

            // Calculate direction and spawn position offset
            Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            // Ensure direction.z is 0 to avoid any z-axis change
            direction.z = 0; // Force z to be zero

            // Adjust spawn position
            Vector3 spawnPosition = transform.position + direction * 0.5f; // Adjust offset as needed

            // Instantiate the card projectile
            GameObject cardProjectile = Instantiate(cardPrefab, spawnPosition, Quaternion.identity);

            // Initialize the projectile with card properties
            cardProjectile.GetComponent<CardProjectile>().Initialize(damage, knockback, speed, bounces, pierce, direction);
            isThrowing = true;

            // Set the card's sprite to the back (e.g., rectangle)
            SpriteRenderer sr = cardProjectile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = sprites[0]; // Set the sprite to the card back
            }

            // Discard the card
            Discard.Add(card);
            Hand.RemoveAt(0);

            // Redraw if Hand is now empty
            if (Hand.Count < 1) Draw();

            isThrowing = false;
        }
    }



    public override void SecondaryAttack()
    {
        if (Hand.Count < 1)
        {
            Debug.LogError("Hand is empty");
            return;
        }

        if (!isThrowing)
        {
            isThrowing = true;

            // Spread throw all cards in hand
            int cardCount = Hand.Count;
            float angleStep = 15f; // Adjust to control spread angle
            float startAngle = -((cardCount - 1) * angleStep) / 2;

            for (int i = 0; i < cardCount; i++)
            {
                Card card = Hand[0];
                int damage = card.GetCardDamage();
                int knockback = card.Knockback;
                float speed = card.Speed;
                int bounces = card.Bounces;
                int pierce = card.Pierce;

                // Instantiate and initialize each card projectile
                GameObject cardProjectile = Instantiate(cardPrefab, transform.position, Quaternion.identity);
                float angle = startAngle + (angleStep * i);
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

                cardProjectile.GetComponent<CardProjectile>().Initialize(damage, knockback, speed, bounces, pierce, direction);

                // Set sprite to the back of the card
                SpriteRenderer sr = cardProjectile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = sprites[0]; // Use the back sprite
                }

                // Discard the card
                Discard.Add(card);
                Hand.RemoveAt(0);
            }

            // Draw the next hand from the deck
            Draw();

            isThrowing = false;
        }
    }
}
