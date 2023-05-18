using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoInimigoMorcegoDrone : MonoBehaviour
{
    private GameObject alvo, controladorGame;
    // Pontos de vida
    public int pontosVida = 1;
    // XP quando morre
    public int xpInimigo = 5;
    // Movimento
    public bool isAutomatic = false, turn = false;
    public float velocidadeMovimento = 2.0f, velocidadeRotacao = 1.0f, anguloZ, atrasoRotacao;
    private float time = 0;
    // materiais inimgo
    private MeshRenderer[] renderers;
    private Material[] materiais;
    // efeito explos�o
    public GameObject fxExplosionPrefab;

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

    private void OnEnable()
    {
        if (controladorGame.GetComponent<ControladorGame>().nivel >= 8)
        {
            pontosVida = 4;
            xpInimigo = 10;
        }
        if (controladorGame.GetComponent<ControladorGame>().nivel >= 10)
        {
            xpInimigo = 0;
        }
    }
    private void Update()
    {
        if (Time.timeScale == 0) return;

        Utilidades.DestroyOutOfScreen(transform.position, gameObject);

        if (isAutomatic)
        {
            MovimentaMorcegoDrone();
        }
    }
    private void MovimentaMorcegoDrone()
    {
        transform.Translate(0, velocidadeMovimento * Time.deltaTime, 0);

        if (turn)
        {
            Coroutine coroutine;
            if (time < velocidadeRotacao + atrasoRotacao + 0.5)
            {
                time += Time.deltaTime;
                coroutine = StartCoroutine(AtrasaRotacao(anguloZ, atrasoRotacao));
                return;
            }
            StopCoroutine(nameof(coroutine));
        }
    }
    private IEnumerator AtrasaRotacao(float angZ, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(RotacaoLerp(new Vector3(0, 0, angZ), velocidadeRotacao));
        yield break;
    }
    private IEnumerator RotacaoLerp(Vector3 valorFinal, float duracao)
    {
        float time = 0;
        Vector3 valorInicial = transform.rotation.eulerAngles;
        valorFinal += valorInicial;
        while (time < duracao)
        {
            transform.eulerAngles = Vector3.Lerp(valorInicial, valorFinal, time / duracao);
            time += Time.deltaTime;
            yield return null;
        }
        transform.eulerAngles = valorFinal;
        StopAllCoroutines();
        yield break;
    }
    private void CaluclaDanoInimigo(int dano)
    {
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
            Instantiate(fxExplosionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            ControladorGame.instancia.SomaXP(xpInimigo);
        }
    }
    private void OnCollisionEnter(Collision colisor)
    {
        if (colisor.gameObject.CompareTag("BalaPersonagem"))
        {
            Destroy(colisor.gameObject);
            int dano = alvo.GetComponent<ControlaPersonagem>().danoArmaPrincipal;

            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("BalaPet"))
        {
            Destroy(colisor.gameObject);
            int dano = alvo.GetComponent<DisparoArmaPet>().danoArmaPet;

            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("OrbeGiratorio"))
        {
            int dano = alvo.GetComponent<RespostaOrbeGiratorio>().danoOrbeGiratorio;

            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("ProjetilSerra"))
        {
            int dano = alvo.GetComponent<DisparoArmaSerra>().danoSerra;

            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("Player"))
        {
            int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;

            CaluclaDanoInimigo(dano);
        }
    }
}

    
