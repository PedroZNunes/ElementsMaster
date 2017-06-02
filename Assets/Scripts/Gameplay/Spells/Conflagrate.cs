using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOT IMPLEMENTED YET.
/// </summary>
public class Conflagrate : Spell {

    /* essa spell gera algum tipo de aviso qnd est[a disponivel.
     * ela faz isso atraves de o recebimento de um evento.
     * um evento que passa a posicao do creep em chamas. OnFire event
     */
    public void Cast () {
        /* quando castado ele busca numa area Radius ao redor do player.
         * se ele achar algum inimigo ele procura nessa lista por inimigos pegando fogo.
         * se tiver algum, colocamos nele o debuff de coflagrateDebuff
         * no awake do coflagrate debuff o cara explode e soma o stack de fogo q ele tiver (como conflagration fire)
         * ao de todos a uma area explosionRadius ao redor dele.
         * conflagration fire n'ao pode ser `conflagrado.`
        */
    }
}
