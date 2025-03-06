;Fecha de Compilación: 04/03/2025
;Hora de Compilación: 12:01:21
SEGMENT .TEXT
GLOBAL MAIN
MAIN:
	mov eax,6
	push eax 
tpop eax
	PRINT_DEC 
	RET
         section .data
x26 dd 0
