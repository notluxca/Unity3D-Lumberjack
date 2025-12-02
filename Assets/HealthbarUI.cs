using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{
    public Image healthBarImage;

    public PlayerController playerController;
    private float maxHealth;
    private float currentHealth;

    /// <summary>
    /// Initializes the health bar UI by setting the maximum health and the current health to the player's health.
    /// It then sets the fill amount of the health bar image to the current health divided by the maximum health.
    /// </summary>
    private void Start()
    {
        maxHealth = playerController.Health;
        currentHealth = maxHealth;
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }

    private void Update()
    {
        currentHealth = playerController.currentHealth;
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }

    public void UpdateHealth(float newHealth)
    {
        currentHealth = newHealth;
        healthBarImage.fillAmount = currentHealth / maxHealth;
    }
}

