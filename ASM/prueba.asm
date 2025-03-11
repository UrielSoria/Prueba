;Fecha de Compilación: 11/03/2025
;Hora de Compilación: 12:22:11
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
;While
While1:
	mov eax, dword[i]
	push eax
	mov eax,10
	push eax 
	pop ebx
	pop eax
    cmp eax, ebx
	ja brinco_While_1
; Incremento de a 
	inc dword[a]
; Incremento de i 
	inc dword[i]
	jmp While1
brinco_While_1:
	RET
         section .data
a dd 0
i dd 0
