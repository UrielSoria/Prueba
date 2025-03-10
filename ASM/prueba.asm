;Fecha de Compilación: 10/03/2025
;Hora de Compilación: 16:37:16
segment .text
global main
main:
	mov edx, 0
	mov eax,0
	push eax 
;Asignacion de a
	pop eax
	mov dword[a], eax
	mov eax,0
	push eax 
;Asignacion de i
	pop eax
	mov dword[i], eax
;for
For2:
	mov eax, dword[i]
	push eax
	mov eax,10
	push eax 
	pop ebx
	pop eax
    cmp eax, ebx
	ja brinco_for_1
; Incremento de i 
	inc dword[i]
; Incremento de a 
	inc dword[a]
	jmp For2
brinco_for_1:
	RET
         section .data
a dd 0
i dd 0
