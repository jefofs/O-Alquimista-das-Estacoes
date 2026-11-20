# Estado do Projeto — O Alquimista das Estações

## Objetivo atual
Evoluir o loop principal da Ilha 1 (`Assets/Scenes/Ilha_01_Primavera.unity`) combinando colheita em arbustos (estilo Stardew Valley), mineração/corte com golpes e feedback visual (estilo Forager) e criação da Picareta na Bancada (estilo Minecraft).

## Feito
- **Movimento, Câmera e Base da Ilha 1:** `ChaoIlha`, `LimitesIlha`, `Player` (`PlayerMovement`) e `Main Camera` (`CameraFollow`).
- **Recursos Expandidos e Ferramenta (`TipoIngrediente.cs`, `InventarioAlquimista.cs`):**
  - Enum expandido: `Fruta`, `Cristal`, `Raiz`, `Madeira`, `Pedra`.
  - Propriedade `TemPicareta`, método `DesbloquearPicareta()` (evento `OnFerramentaAlterada`) e métodos `TemRecursos`, `TentarConsumir` e `ConsumirQuantidade`.
- **Ações do Jogador (`PlayerAcoes.cs` e `Player.prefab`):**
  - **Tecla E:** Interage com o `IInteragivel` mais próximo (`Physics2D.OverlapCircleAll`, raio `1.6`).
  - **Espaço / Clique Esquerdo:** Ataca/minera `RecursoMineravel` à frente (`UltimaDirecao`, cooldown `0.35s`, dano `1` com mãos e `2` com Picareta) com swing visual de `0.15s` no filho `VisualFerramenta`.
- **Arbusto Interativo (`ArbustoFrutas.cs` e `Arbusto_Frutas.prefab`):**
  - Implementa `IInteragivel`, estados `ComFrutas`/`Vazio`, regeneração em `8s`, filho `IndicadorFrutas` e drop de 1 a 2 `Ingrediente_Fruta`.
- **Nós Mineráveis (`RecursoMineravel.cs`) e Drops:**
  - Novos drops: `Ingrediente_Madeira.prefab` e `Ingrediente_Pedra.prefab`.
  - Prefabs sólidos (`Sorting Order = 4`): `Recurso_Arvore.prefab` (vida 3, dropa 2 Madeiras), `Recurso_Rocha.prefab` (vida 3, dropa 2 Pedras) e `Recurso_VeioCristal.prefab` (vida 4, exige Picareta, dropa 2 Cristais) com tremor, flash branco e evento `OnAvisoJogador`.
- **Bancada de Criação (`BancadaCraft.cs` e `BancadaCraft.prefab`):**
  - Posicionada em `(-3, 2.5, 0)`, consome `3 Madeiras` e `2 Pedras` ao interagir (`E`) para desbloquear a Picareta.
- **Mapa da Ilha 1, Spawner e HUD (`SpawnerIngredientes.cs`, `HUDReceita.cs`):**
  - Layout inicial com 3 Arbustos, 3 Árvores, 3 Rochas e 2 Veios de Cristal + respawn gradual com checagem `Physics2D.OverlapCircle`.
  - HUD atualizada com Mochila completa, Ferramenta atual (`Mãos`/`Picareta`), Receita do Caldeirão, painel inferior de Avisos/Prompts (`[E]`) e legenda de controles no topo direito.
- Compilação e Console do Unity verificados via MCP (`0` erros, `0` warnings).

## Próximo passo
- Implementar o `GameManager`, transição de vitória/próxima ilha e mecânicas específicas das 5 estações/ilhas.

## Decisões tomadas
- **Engine & Pipeline:** Unity 6 (2D URP), Física 2D (`OverlapCircleAll`, colliders sólidos nos nós e triggers nos drops/Caldeirão) e New Input System (`Keyboard.current`, `Mouse.current`).
- **Padrões de Código:** Singleton (`InventarioAlquimista`, `Caldeirao`), Observer (`OnInventarioAlterado`, `OnFerramentaAlterada`, `OnReceitaAlterada`, `OnAvisoJogador`, `OnPromptInteracaoAlterado`), Interface (`IInteragivel`) e Prefabs em `Assets/Prefabs/`.
- **UI:** Canvas `1280x720` (`Scale With Screen Size`) com uGUI `Text` (`LegacyRuntime.ttf`).
