;Fecha de Compilación: 10/03/2025
;Hora de Compilación: 15:54:10
segment .text
global main
main:
	mov edx, 0
; Asignacion de i 
	mov eax,0
	push eax 
	pop eax
	mov dword[i],eax
	mov eax,i
	push eax
	mov eax,3
	push eax 
	pop ebx
	pop eax
    cmp eax, ebx
; Incremento de i 
	inc dword [i]
	RET
         section .data
i dd 0
