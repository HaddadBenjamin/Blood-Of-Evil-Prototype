/*
** how_many_percent_life.c for how_many_percent_life in /home/avice_d//Projets
** 
** Made by damien avice
** Login   <avice_d@epitech.net>
** 
** Started on  Thu Mar 28 15:14:04 2013 damien avice
** Last update Thu Mar 28 15:27:37 2013 damien avice
*/


int	how_many_percent_life(int vie_tot, int vie_rest)
{
  float	vie_actu;

  vie_actu = vie_rest / 100;
  printf("%f / %d Sante\n", vie_actu, vie_tot);
  return (vie_actu);
}
