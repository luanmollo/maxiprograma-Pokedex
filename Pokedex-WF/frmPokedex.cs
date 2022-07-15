﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;

namespace Pokedex_WF
{
    public partial class frmPokedex : Form
    {
        private List<Pokemon> listaPokemon;
        public frmPokedex()
        {
            InitializeComponent();
        }

        
        private void frmPokedex_Load(object sender, EventArgs e)
        {
            cargar();
        }

        private void cargar()
        {
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                listaPokemon = negocio.Listar();
                dgvPokemons.DataSource = listaPokemon;
                ocultarColumnas();

                cargarImagen(listaPokemon[0].UrlImagen);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            dgvPokemons.Columns["UrlImagen"].Visible = false;
            dgvPokemons.Columns["Id"].Visible = false;
        }

        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvPokemons.CurrentRow != null)
            {
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
            }
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbPokemon.Load(imagen);
            }
            catch (Exception ex)
            {
                pbPokemon.Load("https://www.sinrumbofijo.com/wp-content/uploads/2016/05/default-placeholder.png");
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaPokemon alta = new frmAltaPokemon();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Pokemon seleccionado;
            seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

            frmAltaPokemon modificar = new frmAltaPokemon(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        private void btnOcultar_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }

        private void eliminar(bool logico = false)
        {
            PokemonNegocio pokemonNegocio = new PokemonNegocio();
            Pokemon seleccionado;

            try
            {
                DialogResult respuesta = MessageBox.Show("¿Estás seguro de eliminar?", "Eliminando...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

                    if (logico)
                    {
                        pokemonNegocio.Ocultar(seleccionado.Id);
                    }
                    else
                    {
                        pokemonNegocio.Eliminar(seleccionado.Id);
                    }

                    cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            List<Pokemon> listaFiltrada;
            string filtro = txtFiltro.Text;

            if(filtro != "")
            {
                listaFiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaPokemon;
            }

            dgvPokemons.DataSource = null;
            dgvPokemons.DataSource = listaFiltrada;
            ocultarColumnas();
        }
    }
}
