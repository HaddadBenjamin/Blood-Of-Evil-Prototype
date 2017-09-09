/*
** Affichage_Vie.c for  in /home/haddad_b//LangageC/BLooDBuRNiNG/GameFunctions
** 
** Made by benjamin haddad
** Login   <haddad_b@epitech.net>
** 
** Started on  Tue Jan  8 11:35:48 2013 benjamin haddad
** Last update Sat Mar 23 15:40:53 2013 benjamin haddad
*/

/* void		affichage_vie(int vie_total, int vie_restante) */
/* { */
/*   double	pourcent_vie; */

/*   pourcent_vie = vie_restante / vie_total * 100; */
/*   if (pourcent_vie == 0,0000) */
/*     affichage_mort; */
/*   else */
/*     affichage_vie_pourcent(pourcent_vie); */
/* } */

double		vie_calcul(int vie_total, int vie_restante)
{
  double	pourcent_vie;

  pourcent_vie = vie_restante / vie_total * 100;
  printf("Il vous reste %f / %d de vie\n", vie_restante, vie_total);
  if (vie_restante <= 0)
    printf("Vous etes mort dans d'attroce souffrance\n");
  else
    printf("Soit %f pourcent de vie, toute mes félicitations vous etes toujours en vie\n", vie_restante, vie_total, pourcent_vie);
  return (pourcent_vie);
}

int		main()
{
  vie_calcul(14545400, 2443);
  vie_calcul(6000, 10002);
  vie_calcul(400, 400);
}


