using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoBossFase2 : MonoBehaviour
{
    private GameObject alvo;
    //vida
    public int primeiraVida = 300, segundaVida = 300;
    private bool stage1 = true, stage2 = false, stage3 = false;

    //tiros
    public GameObject balaRa, balaOlhos, olhoRa, olhoEsq, olhoDir;
        // Ra
        [Range(0, 5)] public float cooldownRa = 0.3f, tempoDisparoRa = 3.0f;
        public int numeroDisparosRa = 10;
        private float contadorCooldownRa, contadorDisparosRa;
        private bool ativaArmaRa = false;
        // Olhos
        [Range(0, 5)] public float cooldownOlhos = 0.3f, tempoDisparoOlhos = 3.0f;
        public int numeroDisparosOlhos = 10;
        private float contadorCooldownOlhos, contadorDisparosOlhos;
        private bool ativaArmaOlhos = false;

    //lasers
    public GameObject laserEsq, laserDir;

    // Materiais
    private MeshRenderer[] renderers;
    private Material[] materiais;
    // fx
    public GameObject fxExplosionPrefab, fxExpHit, fxExpHitPet;

    private void Start()
    {
        alvo = GameObject.FindGameObjectWithTag("Player");
        // Busca materiais do inimigo
        renderers = GetComponentsInChildren<MeshRenderer>();
        materiais = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length - 2; i++)
        {
            materiais[i] = renderers[i].material;
        }

        ativaArmaRa = false;
        StartCoroutine(IntervaloDisparoRa(2.0f));
    }
    private void Update()
    {
        if (stage1)
        {
            ArmaRa();
        }
        
        if (stage2)
        {
            ArmaOlhos();
        }
    }

    // ativa/desativa lasers
    private void AtivaLasers(bool ativa)
    {
        laserDir.SetActive(ativa);
        laserEsq.SetActive(ativa);
    }

    // arma olho de Ra
    private void ArmaRa()
    {
        // disparo arma Ra
        if (ativaArmaRa)
        {
            // Cooldown e controle tiro Ra
            Utilidades.CalculaCooldown(contadorCooldownRa);
            contadorCooldownRa = Utilidades.CalculaCooldown(contadorCooldownRa);
            if (contadorCooldownRa == 0)
            {
                TiroBalaRa();
                contadorCooldownRa = cooldownRa;
                contadorDisparosRa++;
                if (contadorDisparosRa < numeroDisparosRa) return;
                else
                {
                    contadorDisparosRa = 0;
                    ativaArmaRa = false;
                    StartCoroutine(IntervaloDisparoRa(tempoDisparoRa));
                }
            }
        }
    }
    private void TiroBalaRa()
    {
        Instantiate(balaRa, olhoRa.transform.position, olhoRa.transform.rotation);
    }
    private IEnumerator IntervaloDisparoRa(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        ativaArmaRa = true;
    }

    // arma olhos
    private void ArmaOlhos()
    {
        // disparo arma Ra
        if (ativaArmaOlhos)
        {
            // Cooldown e controle tiro Ra
            Utilidades.CalculaCooldown(contadorCooldownOlhos);
            contadorCooldownOlhos = Utilidades.CalculaCooldown(contadorCooldownOlhos);
            if (contadorCooldownOlhos == 0)
            {
                TiroBalaOlhos();
                contadorCooldownOlhos = cooldownOlhos;
                contadorDisparosOlhos++;
                if (contadorDisparosOlhos < numeroDisparosOlhos) return;
                else
                {
                    contadorDisparosOlhos = 0;
                    ativaArmaOlhos = false;
                    StartCoroutine(IntervaloDisparoOlhos(tempoDisparoOlhos));
                }
            }
        }
    }

    private void TiroBalaOlhos()
    {
        Instantiate(balaRa, olhoEsq.transform.position, olhoEsq.transform.rotation);
        Instantiate(balaRa, olhoDir.transform.position, olhoDir.transform.rotation);
    }
    private IEnumerator IntervaloDisparoOlhos(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        ativaArmaRa = true;
    }
    // dano e St�gios
    private void CaluclaDanoInimigo(int dano)
    {
        if (stage1)
        {
            if (primeiraVida > 0)
            {
                primeiraVida -= dano;

                foreach (Material material in materiais)
                {
                    StartCoroutine(Utilidades.PiscaCorRoutine(material));
                }
            }
            if (primeiraVida <= 0)
            {
                stage1 = false;
                stage2 = true;
                AtivaLasers(true);
            }
        }
        
        if (stage2)
        {
            if (segundaVida > 0)
            {
                segundaVida -= dano;

                foreach (Material material in materiais)
                {
                    StartCoroutine(Utilidades.PiscaCorRoutine(material));
                }
            }
            if (segundaVida <= 0)
            {
                stage2 = false;
                stage3 = true;
            }
        }
    }
    private void FXExplosionHit(GameObject fxExpHit, Collision colisor)
    {
        ContactPoint point = colisor.GetContact(0);
        Vector3 pos = point.point;
        Instantiate(fxExpHit, pos, colisor.transform.rotation);
    }
    private void OnCollisionEnter(Collision colisor)
    {
        if (colisor.gameObject.CompareTag("BalaPersonagem"))
        {
            Destroy(colisor.gameObject);
            int dano = alvo.GetComponent<ControlaPersonagem>().danoArmaPrincipal;

            FXExplosionHit(fxExpHit, colisor);
            CaluclaDanoInimigo(dano);
        }
        if (colisor.gameObject.CompareTag("BalaPet"))
        {
            Destroy(colisor.gameObject);
            int dano = alvo.GetComponent<DisparoArmaPet>().danoArmaPet;

            FXExplosionHit(fxExpHitPet, colisor);
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
        if (colisor.gameObject.CompareTag("Escudo"))
        {
            int dano = alvo.GetComponent<ControlaPersonagem>().danoContato;

            CaluclaDanoInimigo(dano);
        }
    }
}
