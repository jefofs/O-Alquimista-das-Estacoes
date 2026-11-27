# Estado do Projeto — O Alquimista das Estações

## Objetivo atual
Implementar o Sistema de Vida/Dano/Cura do Player (`PlayerVida`), combate contra o Inimigo Elemental 2D (`InimigoElemental`), `GameManager` com fluxo de Vitória/Derrota e painéis de UI na cena `Assets/Scenes/Ilha_01_Primavera.unity`.

## Feito
- **Movimento, Coleta, Mineração, Craft e Sprites da Equipe:** Loop completo da Ilha 1 com `PlayerMovement`, `CameraFollow`, `InventarioAlquimista`, `ArbustoFrutas`, `RecursoMineravel`, `BancadaCraft`, `Caldeirao`, `SpawnerIngredientes` e sprites definitivos (`869b225`) preservados.
- **Vida, Dano e Cura do Player (`PlayerVida.cs` e `Player.prefab`):**
  - `_vidaMaxima = 5`, `_tempoInvencivel = 1.0f`, piscar em vermelho durante invencibilidade, métodos `TomarDano` e `RecuperarVida` e eventos `OnVidaAlterada` e `OnPlayerMorreu`.
  - Tecla `F` em `PlayerAcoes.cs` consome 1 `Fruta` para curar `+1 Vida` (com avisos na HUD se a vida estiver cheia ou sem frutas).
- **Combate e Inimigo Elemental 2D (`InimigoElemental.cs` e `Inimigo_SlimeElemental.prefab`):**
  - `PlayerAcoes.cs` expandido para atingir `InimigoElemental` com dano `1` (mãos) ou `2` (Picareta) e knockback direcional.
  - `InimigoElemental.cs`: patrulha simples, perseguição via `Rigidbody2D.linearVelocity` dentro de `_raioDeteccao = 6f`, dano de contato com recarga (`1.2s`), flash de dano, knockback e drop ao morrer.
  - 2 instâncias de `Inimigo_SlimeElemental.prefab` posicionadas próximas aos Veios de Cristal na Ilha 1.
- **GameManager e Painéis de Vitória/Derrota (`GameManager.cs`, `HUDReceita.cs`):**
  - Singleton `GameManager.Instance` inscrito em `Caldeirao.OnReceitaCompleta` e `PlayerVida.OnPlayerMorreu`, controlando `EstadoJogo`, `Time.timeScale` e métodos `ReiniciarFase`, `CarregarProximaFase` e `VoltarAoMenu`.
  - `HUDReceita.cs` atualizado com `TextoVida` (`Vida: 5/5`), legenda com `F: Comer Fruta (+1 Vida)`, `PainelVitoria` (`"Ilha Purificada!"`) e `PainelDerrota` (`"Você foi derrotado!"`).
- Compilação e Console do Unity verificados via MCP (`0` erros, `0` warnings).

## Próximo passo
- Criar as cenas das próximas ilhas/estações (Ilha 2 a 5), configurar `_nomeProximaCena` no `GameManager` e expandir receitas e mecânicas sazonais.

## Decisões tomadas
- **Engine & Pipeline:** Unity 6 (2D URP), Física 2D (`Rigidbody2D.linearVelocity`, `OverlapCircleAll`) e New Input System (`Keyboard.current`, `Mouse.current`).
- **Padrões de Código:** Singleton (`GameManager`, `InventarioAlquimista`, `Caldeirao`, `PlayerVida`), Observer (`event Action`) e Prefabs em `Assets/Prefabs/`.
- **Workflow Git/GitHub:** Desenvolvimento em branches (`feat/inimigo-vida-gamemanager`), nunca direto na `main`, com PR via `gh pr create`.
