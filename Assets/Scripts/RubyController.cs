using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;

    public int cogs = 4;
    
    
    public GameObject projectilePrefab;
    public GameObject ProjectilePrefab2;
    
    public AudioClip jambiSound;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip Losssound;
    public AudioClip Winsound;
    public AudioClip Npcsound;
    public AudioClip Robotsound;
    public AudioClip Finalwinsound;
    public AudioSource musicSource;
    public AudioSource source;
    public AudioSource sfxSource;
    
    public AudioClip collectedCogClip;
    public AudioClip collectedHealthClip;
    public AudioClip collectedGemClip;
    public AudioClip collectedFeatherClip;

    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;

    public static int level = 1;


    
    public GameObject winText;
    public GameObject lostmessage;
    public Text scoreText;
    public Text cogText;
    public Text gemsText;
    public GameObject jambiText;


    
    
    public ParticleSystem healthParticles;
    public ParticleSystem damageParticles;
    public ParticleSystem cogParticles;
    public ParticleSystem gemParticles;
    public ParticleSystem speedParticles;
    public ParticleSystem featherParticles;

    private EnemyController enemyController;

    public int gems;
    public int feathers;

    public int score;
    public bool gameOver = false;
    public GameObject player;

    private bool playedVictory = false;
    private bool playedLoss = false;
    private bool playedVictory2 = false;

    public float featherDuration = 5f;
    public float speedDuration =5f;

    
    
    

    
    

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        

        
        audioSource = GetComponent<AudioSource>();
        winText.SetActive(false);
        
        lostmessage.SetActive(false);

        jambiText.SetActive(false);

        gems = 0;

        feathers = 0;
    

        score = 0;

        cogs  =  4;

        Debug.Log("Current level: " + level);
        
        
        SetCogText();
        SetScoreText();
        SetGemsText();
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
       

        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            cogs = cogs + 4;
            SetCogText();
            PlaySound(collectedCogClip);
            Instantiate(cogParticles, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        }

        if (other.gameObject.CompareTag("Gems"))
        {
            other.gameObject.SetActive(false);
            gems = gems + 1;
            SetGemsText();
            PlaySound(collectedGemClip);
            Instantiate(gemParticles, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            
        }

        if (other.gameObject.CompareTag("Feather"))
        {
            other.gameObject.SetActive(false);
            feathers = feathers + 1;
            PlaySound(collectedFeatherClip);
            Instantiate(featherParticles, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            featherDuration = 5f;
            

        }
        
      
    }

    

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <=0)
        {
            gameOver = true;
            lostmessage.SetActive(true);
            speed = 0;  
        }
        
        if (SceneManager.GetActiveScene().name == "Main2" && score >= 5 && gems == 4)
        {
            
            Debug.Log("Game over condition met!");
            gameOver = true;
            winText.SetActive(true);
            speed = 0;
            
            
        }



        

        if (feathers >= 1)
        {
            featherDuration -= Time.deltaTime;

            if (featherDuration > 0f)
            {
                speed = 8;
                Instantiate(speedParticles, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            }
            else
            {
                featherDuration = 0f; // Reset speed duration to its original value
                feathers = 0; 
                speed = 5; 
                speedParticles.Stop(); 
            }

            if (gameOver)
            {
                speed = 0;
            }       
        }

        
    
        
        if (!playedVictory && (score == 5))
        {
            
            playedVictory = true;
            musicSource.Stop();
            sfxSource.PlayOneShot(Winsound);
            jambiText.SetActive(true);
            
        }

        if (!playedVictory2 && (SceneManager.GetActiveScene().name == "Main2" && score >= 5 && gems == 4))
        {
            
            playedVictory2 = true;
            musicSource.Stop();
            sfxSource.PlayOneShot(Finalwinsound);
            source.Stop();

        }

        

        if (!playedLoss && (health <= 0))
        {
            
            playedLoss = true;
            musicSource.Stop();
            sfxSource.PlayOneShot(Losssound);
            source.Stop();
        }
        
        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        
       
        
        

        

    


        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            
            if(cogs >= 1)
            {
                Launch();

                cogs--;

                SetCogText();
            }
            
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (score >= 5)
                {
                    SceneManager.LoadScene("Main2");
                    
                }
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    PlaySound(jambiSound);
                }

               
                
                
            }
            RaycastHit2D hit2 = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC2"));
            if (hit2.collider != null)
            {

                SlimeQuestScript slime = hit2.collider.GetComponent<SlimeQuestScript>();
                if (slime != null)
                {
                    slime.DisplayDialog();
                    PlaySound(Npcsound);
                }
                

                
                
            }
            
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");
            Application.Quit();
        }

        Debug.Log(currentHealth + "/" + maxHealth);

         
        
    }


    void SetCogText()
    {
        cogText.text = "Cogs:"+ cogs.ToString();
    }

    void SetScoreText()
    {
        scoreText.text = "Robots Fixed: " + score.ToString();
    }
    void SetGemsText()
    {
        gemsText.text = "Gems: " + gems.ToString();
    }


    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;

            Instantiate(damageParticles, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            PlaySound(hitSound);
        }
        else
        {
            Instantiate(healthParticles, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(collectedHealthClip); 
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeScore(int scoreAmount)
    {
        score += scoreAmount;

        // Update the UI text
        scoreText.text = "Robots Fixed: " + score;
        PlaySound(Robotsound);

    }


    

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        
        PlaySound(throwSound);
        animator.SetTrigger("Launch");

        
    } 
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }


}