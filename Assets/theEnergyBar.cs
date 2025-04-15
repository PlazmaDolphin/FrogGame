using UnityEngine;
using TMPro;

public class theEnergyBar : MonoBehaviour
{
    public Transform energyBar, hintBar;
    public AudioSource croakSFX;
    public TextMeshProUGUI energyText;
    public bool isActive = false; // Indicates if the energy bar is being used
    public float energy = 1f, hint = 1f, prevLvl = 1f; // Current energy level (0 to 1)
    private float croakPower = 0.12f;
    private float croakCooldown = 0.4f;
    private float croakStartTime = 0f;
    private bool croaking = false; // Indicates if the croak action is in progress
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setActive(bool active)
    {
        isActive = active; // Set the active state of the energy bar
    }
    public bool useEnergy(float amount)
    {
        bool enough = energy-amount < 0f;
        energy -= amount; // Decrease energy by the specified amount
        if (energy < 0f) energy = 0f;
        hint = energy; // Decrease hint by the same amount
        return enough; // Energy used successfully
    }
    public void slideEnergy(float percent){
        energy = hint + (prevLvl-hint)*(1f-percent);
    }
    public bool useHint(float amount){
        if (energy-amount < 0f) return false; // Not enough energy for hint
        hint = energy-amount;
        prevLvl = energy; // Store the previous energy level
        return true; // Hint used successfully
    }
    public void gainEnergy(float amount)
    {
        energy += amount; // Increase energy by the specified amount
        if (energy > 1f) energy = 1f; // Clamp energy to a maximum of 1
        hint = energy; // Set hint to the current energy level
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        energyBar.localScale = new Vector3(1f, energy, 1f); // Update the energy bar scale based on current energy
        hintBar.localScale = new Vector3(1f, hint, 1f); // Update the hint bar scale based on current hint
        // Listen for RMB to croak (gain energy)
                // Give control back when done croaking
        if (Time.time - croakStartTime >= croakCooldown && croaking){
            croaking = false; // Reset the croaking state
            setActive(false); // Set the energy bar to inactive state
            // If RMB still held, allow croaking again
        }
        else if (croaking){
            setActive(true); // Keep the energy bar active while croaking
        }
        if (Input.GetMouseButton(1) && !isActive && energy < 0.999f){
            croak();
        }
        //set text color green if 90% or more, red if empty
        if (energy >= 0.9f) energyText.color = new Color(0.2f, 1f, 0.2f, 1f); // Green color for high energy
        else if (energy <= 0.1f) energyText.color = new Color(1f, 0.2f, 0.2f, 1f); // Red color for low energy
        else energyText.color = new Color(1f, 1f, 1f, 1f); // White color for normal energy level
    }

    private void croak(){
            croakStartTime = Time.time; // Record the time when the croak starts
            gainEnergy(croakPower); // Gain energy when croaking
            setActive(true); // Set the energy bar to active state
            croaking = true; // Set the croaking state to true
            if (croakSFX != null) croakSFX.Play(); // Play the croak sound effect
    }
}
