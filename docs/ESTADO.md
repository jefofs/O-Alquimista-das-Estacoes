# Estado do Projeto — O Alquimista das Estações

## Objetivo atual
Implementar o Movimento Top-Down 2D do Player e o sistema de Câmera suave (`CameraFollow`) na cena base da Ilha 1 (`Assets/Scenes/Ilha_01_Primavera.unity`).

## Feito
- Conexão MCP validada, estrutura de pastas em `Assets/` criada e diretrizes definidas em `AGENTS.md`.
- Criada e configurada a cena `Assets/Scenes/Ilha_01_Primavera.unity` (adicionada ao Build Settings):
  - `ChaoIlha`: Sprite 2D quadrado (`Assets/Sprites/Square.png`), cor verde claro de primavera, escala `(20, 14, 1)` e `Sorting Order = -10`.
  - `LimitesIlha`: Objeto pai vazio com 4 filhos (`Norte`, `Sul`, `Leste`, `Oeste`) equipados com `BoxCollider2D` alinhados às bordas do chão (`X: ±10`, `Y: ±7`).
  - `Player`: Posicionado em `(0, 0, 0)` com Tag `Player`, `SpriteRenderer` (roxo/azul, `Sorting Order = 5`), `Rigidbody2D` (`Dynamic`, `Gravity Scale = 0`, `Freeze Rotation Z`, `Interpolate`), `BoxCollider2D` e `PlayerMovement`.
- Criado o Prefab `Assets/Prefabs/Player.prefab` conectado à instância na cena.
- Criado `Assets/Scripts/PlayerMovement.cs`:
  - Leitura de WASD e Setas via `Keyboard.current` (New Input System), vetor normalizado, `_velocidade = 5f`, `linearVelocity` no `FixedUpdate`, espelhamento `flipX` e propriedade pública `UltimaDirecao`.
- Criado `Assets/Scripts/CameraFollow.cs` e anexado à `Main Camera`:
  - Fundo da câmera em azul água suave (mar), `_alvo` apontando para o `Player` (com fallback `FindAnyObjectByType<PlayerMovement>()`), `_suavizacao = 6f` e `Vector3.Lerp` no `LateUpdate` mantendo `Z = -10`.
- Compilação e Console do Unity verificados via MCP (`0` erros, `0` warnings).

## Próximo passo
- Implementar os sistemas de estações/alquimia, itens coletáveis (`OnTriggerEnter2D`), Managers (`GameManager`) com padrão Singleton/Observer e HUD.

## Decisões tomadas
- **Engine & Pipeline:** Unity 6 (2D URP), usando `Rigidbody2D.linearVelocity` e `FindAnyObjectByType`.
- **Física & Controles:** Física 2D (`Rigidbody2D` Dynamic sem gravidade + `BoxCollider2D`) e `UnityEngine.InputSystem` (`Keyboard.current`).
- **Padrões de Código:** Scripts em `Assets/Scripts/` seguindo Singleton, Observer e Prefabs (`Assets/Prefabs/Player.prefab` reutilizável nas 5 ilhas).
- **UI:** Padrão único em Canvas (`Scale With Screen Size`, `1280x720`, TextMeshPro) ou UI Toolkit.
- **Workflow Unity/Git:** Edição de cenas/prefabs exclusivamente via MCP com leitura prévia do estado da cena e atualização de `docs/ESTADO.md` a cada entrega.
