# R3 Reset Tool (USB)

Pequeno utilitário para ler e aplicar configurações básicas no leitor UHF (Chainway/derivados) usando `UHFAPI.dll` via USB.

## O que ele faz
- Abre conexão USB (`UsbOpen`)
- Lê e/ou ajusta:
  - **Power** (5–30)
  - **Region** (China1/China2/Europe/USA/Korea/Japan)
  - **RFLink** (índice numérico)
- (Opcional) aplica `SoftReset`

## Requisitos
Na mesma pasta do `R3ResetTool.exe` precisam existir:
- `UHFAPI.dll`
- `hidapi.dll`

## Exemplos
Somente ler:

```bat
R3ResetTool.exe --get
```

Aplicar padrão Brasil (FCC/USA, power 30):

```bat
R3ResetTool.exe --region usa --power 30 --rflink 0 --save 1
```

## Notas
- `--save 1` grava no módulo (persistente). `--save 0` aplica temporário.
- Valores de Region:
  - china1=0x01
  - china2=0x02
  - europe=0x04
  - usa=0x08
  - korea=0x16
  - japan=0x32
