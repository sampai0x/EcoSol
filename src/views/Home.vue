<template>

  <body>
    <section class="landing">
      <!-- navbar -->
      <nav class="navbar">
        <div class="logo" @click="toggleNavbar">
          <img src="/src/img/EcoSolNavBar.png" alt="Logo" class="logo-img" />
          <svg class="menu-icon" xmlns="http://www.w3.org/2000/svg" height="24" width="24" viewBox="0 0 24 24">
            <path d="M4 6h16M4 12h16M4 18h16" stroke="#000" stroke-width="2" stroke-linecap="round" />
          </svg>
        </div>

        <transition name="slide-fade">
          <div v-if="navbarVisible" class="navbar-left">
            <div class="navbar-links">
              <ul>
                <li><a href="#">Início</a></li>
                <li><a href="#">Sobre</a></li>
                <li><a href="#">Contato</a></li>
              </ul>
            </div>
            <form @submit.prevent="login" class="login-form">
              <input type="email" v-model="email" placeholder="Email" required />
              <input type="password" v-model="senha" placeholder="Senha" required />
              <button type="submit">Entrar</button>
            </form>
          </div>
        </transition>
      </nav>

      <img src="/src/img/EcoSol.png" alt="logo" class="solar-image">

      <p>Você deseja se cadastrar como:</p>
      <div class="botoes">
        <button @click="irPara('fornecedor')">Sou Fornecedor</button>
        <button @click="irPara('cliente')">Sou Cliente</button>
      </div>
      <p class="login-link">
        Já tem uma conta? <a href="#" @click.prevent="mostrarLogin">Faça login</a>
      </p>
      <!-- meio da pagina -->
      <div class="meio">
        <div>
          <i class="fas fa-leaf fa-2x" style="color:#16a34a;"></i>
          <h1 id="sobre">Sobre o Projeto</h1>
          <p>
            Somos uma empresa com um projeto que propõe o desenvolvimento de uma plataforma digital para gerenciamento
            de energia renovável...
          </p>
        </div>
        <div>
          <i class="fas fa-solar-panel fa-2x" style="color:#f59e0b;"></i>
          <h1>Compartilhe Energia Solar</h1>
          <p>
            Conecte-se com outras pessoas para fornecer ou receber energia solar de forma simples e colaborativa.
          </p>
        </div>
      </div>

    </section>
  </body>
</template>

<script>
import { useRouter } from 'vue-router'

export default {
  data() {
    return {
      navbarVisible: false,
      email: '',
      senha: '',
    };
  },
  setup() {
    const router = useRouter();
    const irPara = (destino) => {
      router.push(`/${destino}`);
    };
    return { irPara };
  },
  methods: {
    toggleNavbar() {
      this.navbarVisible = !this.navbarVisible;
    },
    mostrarLogin() {
      this.navbarVisible = true;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    },
    login() {
      console.log('Email:', this.email, 'Senha:', this.senha);
    },
  },
};
</script>

<style scoped>
/* landing  */
.landing {
  min-height: 100vh;
  padding: 2rem;
  background: linear-gradient(to bottom, #fef3c7, #fde68a);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
}

.landing h1 {
  font-size: 2.5rem;
  color: #92400e;
  margin-bottom: 1rem;
}

.landing p {
  max-width: 600px;
  font-size: 1.1rem;
  color: #4b5563;
  margin-bottom: 1.5rem;
}

/* navbar */

.navbar {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  background-color: #ffffff;
  padding: 1rem 2rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 1000;
  box-sizing: border-box;
  overflow-x: auto;
  flex-wrap: nowrap;
}

.logo {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.logo-img {
  height: 40px;
  margin-right: 10px;
}

.menu-icon {
  width: 30px;
  height: 30px;
  margin-left: 10px;
  cursor: pointer;
}

.navbar-left {
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 2rem;
  background-color: #ffffff;
  padding: 1rem 2rem;
  border-radius: 8px;
  max-width: 100%;
  box-sizing: border-box;
  flex-wrap: wrap;
}

.navbar-links ul {
  list-style: none;
  display: flex;
  gap: 2rem;
  padding: 0;
  margin: 0;
}


.navbar-links li a {
  text-decoration: none;
  color: #92400e;
  font-weight: bold;
  white-space: nowrap;
}

.navbar-links li a:hover {
  color: #f59e0b;
}

.slide-fade-enter-active {
  transition: all 0.4s ease;
}

.slide-fade-leave-active {
  transition: all 0.3s ease;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
  transform: translateY(-10px);
  opacity: 0;
}

.menu-icon {
  width: 30px;
  height: 30px;
  margin-left: 10px;
  cursor: pointer;
}

.navbar .menu {
  list-style: none;
  display: flex;
  gap: 1.5rem;
}

.navbar .menu li {
  cursor: pointer;
  font-weight: 500;
  color: #92400e;
}

.navbar .menu li:hover {
  color: #f59e0b;
}

/* login navbar  */

.login-form {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.login-form input {
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  box-sizing: border-box;
}

.login-form button {
  background-color: #f59e0b;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  cursor: pointer;
  font-weight: bold;
  transition: background 0.3s;
  white-space: nowrap;
}

.login-form button:hover {
  background-color: #d97706;
}

.login-link {
  margin-top: 1.5rem;
  color: #6b7280;
  font-size: 0.95rem;
}

.login-link a {
  color: #f59e0b;
  text-decoration: underline;
  cursor: pointer;
}

/* garantia de resposividade  */
@media (max-width: 768px) {
  .navbar-left {
    flex-direction: column;
    align-items: stretch;
  }

  .navbar-links ul {
    justify-content: center;
    flex-wrap: wrap;
  }

  .login-form {
    flex-direction: column;
    align-items: stretch;
    width: 100%;
  }

  .login-form input,
  .login-form button {
    width: 100%;
  }
}

/* meio */
.meio {
  display: flex;
  justify-content: center;
  background-color: #ffffff;
  width: 100%;
  gap: 2rem;
  padding: 2rem;
  text-align: center;
  flex-wrap: wrap;
}

.meio i {
  margin-bottom: 1rem;
}

.meio>div {
  max-width: 400px;
}



/* Keyframes para animação dos botões*/

@keyframes pulse {
  0% {
    transform: scale(1);
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  }

  50% {
    transform: scale(1.1);
    box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
  }

  100% {
    transform: scale(1);
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  }
}

@keyframes waveEffect {
  0% {
    transform: scale(1);
    background-color: #f59e0b;
  }

  50% {
    transform: scale(1.1);
    background-color: #d97706;
  }

  100% {
    transform: scale(1);
    background-color: #f59e0b;
  }
}



/* Botões */
.botoes {
  display: flex;
  flex-direction: column;
  gap: 2rem;
  margin-top: 2rem;
  margin-bottom: 2rem;
  align-items: center;
}

/* Garantia de resposividade com os botões */
@media (min-width: 640px) {
  .botoes {
    flex-direction: row;
    justify-content: center;
  }
}

.landing button {
  width: 200px;
  padding: 1rem;
  font-size: 1rem;
  background-color: #f59e0b;
  color: white;
  border: none;
  border-radius: 9999px;
  cursor: pointer;
  transition: all 0.3s ease;
}


.landing button:hover {
  background-color: #d97706;
  animation-name: pulse;
  animation-duration: 1.5s;
  animation-timing-function: ease-in-out;
  animation-iteration-count: infinite;
  transform: scale(1.05);
}

.landing button:active {
  animation-name: waveEffect;
  animation-duration: 0.4s;
  animation-timing-function: ease-in-out;
  animation-iteration-count: 1;
  transform: scale(0.98);
  box-shadow: 0px 2px 5px rgba(0, 0, 0, 0.2);
}


/* imagem */
.solar-image {
  width: 500px;
  margin-top: 2rem;
}
</style>
