using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MovimentoInimigoPiramide : MonoBehaviour
{
    private GameObject alvo, controladorGame;
    public GameObject cabecaPiramide, pontaArma, balaPiramide;
    // Controle rota�ao
    public float velocidadeRotacao = 2.0f;
    public bool petBoss = false;
    // Pontos de vida
    public int pontosVida = 3;
    public int danoTiro = 1;
    // XP quando morre
    public int xpInimigo = 100;
    // Tiro
    [Range(0, 3)] public float cooldown = 1.0f;
    private float contadorCooldown;
    public bool inverteRotacaoTiro = false;
    // materiais inimgo
    private MeshRenderer[] renderers;
    private Material[] materiais;
    // sons inimigo
    public AudioSource inimigoMorre;
    private void Awake()
    {
        controladorGame = GameObject.FindGameObjectWithTag("ControladorGame");
        alvo = GameObject.FindGameObjectWithTag("Player");
        // Busca materiais do inimigo
        renderers = GetComponentsInChildren<MeshRenderer>();
        materiais = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materiais[i] = renderers[i].material;
        }
    }
    void Start()
    {
        contadorCooldown = 4.0f;
    }
    private void OnEnable()
    {
        if(controladorGame.GetComponent<ControladorGame>().nivel >= 8)
        {
            pontosVida = 12;
            xpInimigo = 130;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        if (!petBoss)
        {
            MovimentaInimigoPiramide();
        }
         if (petBoss)
        {
            MovimentaInimigoPiramideBossPet();
        }

        // Cooldown e controle tiro
        Utilidades.CalculaCooldown(contadorCooldown);
        contadorCooldown = Utilidades.CalculaCooldown(contadorCooldown);
        if (contadorCooldown == 0)
        {
            Tiro();
            contadorCooldown = cooldown;
        }
    }

    private void OnCollisionEnter(Collision colisor)
    {
        if (colisor.gameObject.CompareTag("BalaPersonagem"))
        {
            Destroy(colisor.gameObject);
            int dano = alvo.GetComponent<ControlaPersonagem>().danoArmaPrincipal;
            if (pontosVida > 0)
            {
                pontosVida -= dano;

                foreach (Material material in materiais)
                {
                    StartCoroutine(Utilidades.PiscaCorRoutine(material));
                }
            }
            if (pontosVida <= 0)
            {
                inimigoMorre.Play();
                Destroy(gameObject);
                ControladorGame.instancia.SomaXP(xpInimigo);
            }
        }
        if (colisor.gameObject.CompareTag("BalaPet"))
        {
            Destroy(colisor.gameObject);
            int dano = alvo.GetComponent<DisparoArmaPet>().danoArmaPet;
            if (pontosVida > 0)
            {
                pontosVida -= dano;

                foreach (Material material in materiais)
                {
                    StartCoroutine(Utilidades.PiscaCorRoutine(material));
                }
            }
            if (pontosVida <= 0)
            {
                inimigoMorre.Play();
                Destroy(gameObject);
                ControladorGame.instancia.SomaXP(xpInimigo);
            }
        }
        if (colisor.gameObject.CompareTag("OrbeGiratorio"))
        {
            int dano = alvo.GetComponent<RespostaOrbeGiratorio>().danoOrbeGiratorio;
            if (pontosVida > 0)
            {
                pontosVida -= dano;

                foreach (Material material in materiais)
                {
                    StartCoroutine(Utilidades.PiscaCorRoutine(material));
                }
            }
            if (pontosVida <= 0)
            {
                inimigoMorre.Play();
                Destroy(gameObject);
                ControladorGame.instancia.SomaXP(xpInimigo);
            }
        }
        if (colisor.gameObject.CompareTag("ProjetilSerra"))
        {
            int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;
            if (pontosVida > 0)
            {
                pontosVida -= dano;

                foreach (Material material in materiais)
                {
                    StartCoroutine(Utilidades.PiscaCorRoutine(material));
                }
            }
            if (pontosVida <= 0)
            {
                inimigoMorre.Play();
                Destroy(gameObject);
                ControladorGame.instancia.SomaXP(xpInimigo);
            }
        }
        if (colisor.gameObject.CompareTag("Player"))
        {
            int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;
            if (pontosVida > 0)
            {
                pontosVida -= dano;

                foreach (Material material in materiais)
                {
                    StartCoroutine(Utilidades.PiscaCorRoutine(material));
                }
            }
            if (pontosVida <= 0)
            {
                inimigoMorre.Play();
                Destroy(gameObject);
                ControladorGame.instancia.SomaXP(xpInimigo);
            }
        }
    }

    private void MovimentaInimigoPiramide()
    {
        // Rotacao corpo
        Vector3 direcao = alvo.transform.position - transform.position;
        direcao = direcao.normalized;
        transform.up = Vector3.Slerp(transform.up, -1 * direcao, velocidadeRotacao * Time.deltaTime);
        // Mira cabeca
        //cabecaPiramide.transform.up = Vector3.Slerp(cabecaPiramide.transform.up, -1 * direcao, 3 * velocidadeRotacao * Time.deltaTime);
        cabecaPiramide.transform.rotation = Quaternion.LookRotation(cabecaPiramide.transform.forward, - direcao);
        pontaArma.transform.rotation = Quaternion.LookRotation(pontaArma.transform.forward, direcao);
    }

    private void MovimentaInimigoPiramideBossPet()
    {
        Vector3 direcao = alvo.transform.position - cabecaPiramide.transform.position;
        direcao = direcao.normalized;
        cabecaPiramide.transform.rotation = Quaternion.LookRotation(cabecaPiramide.transform.forward, - direcao);
    }

    // Tiro
    private void Tiro()
    {
        if (inverteRotacaoTiro)
        {
            Instantiate(balaPiramide, pontaArma.transform.position, pontaArma.transform.rotation);
        }
        else
        {
            GameObject instaciaBala = Instantiate(balaPiramide, pontaArma.transform.position, pontaArma.transform.rotation);
            instaciaBala.GetComponent<BalaPersonagem>().velocidadeRotacao *= -1;
        }
    }
}