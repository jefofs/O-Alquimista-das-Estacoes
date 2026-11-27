# Estado do Projeto — O Alquimista das Estações

## Objetivo atual
Implementar o fluxo completo de cenas (`Menu.unity` -> `Ilha_01_Primavera.unity` -> `VitoriaFinal.unity`), `MenuManager.cs`, `PainelPausa` (`ESC`) e prefabs reutilizáveis (`GameManager.prefab` e `Canvas_Gameplay.prefab`) para as 5 fases.

## Feito
- **Base, Loop de Gameplay, Combate e Sprites da Equipe (`Ilha_01_Primavera.unity`):**
  - `PlayerMovement`, `CameraFollow`, `InventarioAlquimista`, `PlayerAcoes` (`E`, `Espaço/Clique`, `F` para curar), `PlayerVida`, `ArbustoFrutas`, `RecursoMineravel`, `BancadaCraft`, `Caldeirao`, `SpawnerIngredientes`, `InimigoElemental` e sprites definitivos (`869b225`) preservados.
- **Ilha 1 Preparada para Reutilização (`GameManager.cs`, `GameManager.prefab`, `Canvas_Gameplay.prefab`):**
  - `EventSystem` com `InputSystemUIInputModule`.
  - Botão `"Menu Principal"` adicionado ao `PainelVitoria` e ao `PainelDerrota` (`GameManager.VoltarAoMenu()`).
  - Criado `PainelPausa` ativado pela tecla `ESC` durante `EstadoJogo.Jogando` (`Time.timeScale = 0f`) com botões `"Continuar"`, `"Reiniciar Fase"` e `"Menu Principal"`.
  - `CarregarProximaFase()` carrega `_nomeProximaCena` (quando configurado) ou `"VitoriaFinal"` ao concluir a última fase.
  - Salvos os Prefabs reutilizáveis `Assets/Prefabs/GameManager.prefab` e `Assets/Prefabs/Canvas_Gameplay.prefab`.
- **Cena de Menu Inicial (`Assets/Scenes/Menu.unity` e `Assets/Scripts/MenuManager.cs`):**
  - `MenuManager.cs` com `BtnStartGame()`, `BtnAbrirControles()`, `BtnFecharControles()`, `BtnVoltarMenu()` e `BtnQuit()`.
  - `Canvas` (`1280x720`, `Scale With Screen Size`), fundo temático das 4 estações (`stretch`), título `"O Alquimista das Estações"`, subtítulo, botões `"Jogar"`, `"Como Jogar"` e `"Sair"` e `PainelControles` com instruções e botão `"Voltar"`.
- **Cena de Vitória Final e Créditos (`Assets/Scenes/VitoriaFinal.unity`):**
  - `Canvas` (`1280x720`) com título `"Todas as Estações Foram Restauradas!"`, mensagem de parabéns, painel editável de **Créditos da Equipe e Assets** e botões `"Jogar Novamente"` e `"Menu Principal"`.
- **Build Profile (Scene List) e Validação:**
  - Ordem configurada: `0: Assets/Scenes/Menu.unity`, `1: Assets/Scenes/Ilha_01_Primavera.unity`, `2: Assets/Scenes/VitoriaFinal.unity`.
  - Console do Unity verificado via MCP (`0` erros, `0` warnings).

## Próximo passo
- Criar as cenas das Ilhas 2 a 5 (`Verão`, `Outono`, `Inverno` e `Ilha Final`) reutilizando `Player.prefab`, `Caldeirao.prefab`, `GameManager.prefab` e `Canvas_Gameplay.prefab` e encadeando `_nomeProximaCena`.

## Decisões tomadas
- **UI & Consistência:** Uso padronizado de `Canvas` (`1280x720`, `Scale With Screen Size`) com `UnityEngine.UI.Text` (`LegacyRuntime.ttf`) e `InputSystemUIInputModule` em todas as cenas.
- **Workflow Git/GitHub:** Desenvolvimento em branches por etapa (`feat/menus-fluxo-cenas`) com PR para `main`.
