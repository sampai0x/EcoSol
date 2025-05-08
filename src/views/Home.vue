<template>
  <body>
    <section class="landing">
      <img src="/public/img/EcoSol.png" alt="logo" class="solar-image">
      <h1>Compartilhe Energia Solar</h1>
      <p>
        Conecte-se com outras pessoas para fornecer ou receber energia solar de forma simples e colaborativa.
      </p>
      <p>Você deseja se cadastrar como:</p>
      <div class="botoes">
        <button @click="irPara('fornecedor')">Sou Fornecedor</button>
        <button @click="irPara('cliente')">Sou Cliente</button>
      </div>

    </section>git branch
  </body>
</template>
  
<script setup>
import { useRouter } from 'vue-router'
const router = useRouter()

function irPara(destino) {
  router.push(`/${destino}`)
}

</script>

<style scoped>


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

/* Keyframes para animação */

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
  align-items: center;
}

/* Garantia de resposividade com os botões */
@media (min-width: 640px) {
  .botoes{
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
