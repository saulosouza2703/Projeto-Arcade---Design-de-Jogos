using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorGame : MonoBehaviour
{
    public static ControladorGame instancia;
    // XP e nivel
    public int XP, nivel = 1;
    public float multiplicadorQuantidadeXPporNivel = 50.0f, valorXPNivel = 100.0f;
    public GameObject barraXP, jogador;
    public TextMeshProUGUI txtNivel, txtXP;
    // Power UP
    public GameObject uiPowerUP;
    public bool armaPetAtivada = false;

    private void Awake()
    {
        txtNivel = GameObject.Find("txtN�vel").GetComponent<TextMeshProUGUI>();
        txtXP = GameObject.Find("txtXP").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    
    void Update()
    {
        CalculaBarraXP();
    }

    public void SomaXPBarra(int xpInimigo)
    {
        barraXP.GetComponent<Slider>().value += xpInimigo;
    }
    public void SomaXP(int xpInimigo)
    {
        XP += xpInimigo;
    }

    private void CalculaBarraXP()
    {
        Slider sliderXP = barraXP.GetComponent<Slider>();
        float valorMaxSlider = sliderXP.maxValue;
        float valorSlider = sliderXP.value;
        if (valorSlider == valorMaxSlider)
        {
            SubirNivel();
            if (XP - valorXPNivel == 0)
            {
                sliderXP.value = 0;
                sliderXP.maxValue += (nivel * multiplicadorQuantidadeXPporNivel);
                valorXPNivel += sliderXP.maxValue;
            }
            if (XP - valorXPNivel > 0)
            {
                float diferenca = XP - valorXPNivel;
                sliderXP.value = 0;
                sliderXP.value += diferenca;
                sliderXP.maxValue += (nivel * multiplicadorQuantidadeXPporNivel);
                valorXPNivel += sliderXP.maxValue;
            }
        }
        txtNivel.text = "N�vel: " + nivel;
        txtXP.text = XP + "/" + valorXPNivel;
    }

    private void SubirNivel()
    {
        nivel += 1;
        uiPowerUP.SetActive(true);
        Time.timeScale = 0.0f;
    }
    
    public void PowerUPAtivaArmaNova(GameObject armaNova)
    {
        armaNova.SetActive(true);
        armaPetAtivada = true;
        jogador.GetComponent<DisparoArmaPet>().enabled = true;
        uiPowerUP.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void PowerUPAumentaDanoArmaPrincipal()
    {
        jogador.GetComponent<DisparoArma>().danoArmaPrincipal *= 1.15f;
        uiPowerUP.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void PowerUPDiminuiCooldownArmaPrincipal()
    {
        jogador.GetComponent<DisparoArma>().cooldown *= 0.90f;
        uiPowerUP.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
