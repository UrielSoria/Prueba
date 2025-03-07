;Fecha de Compilación: 07/03/2025
;Hora de Compilación: 11:34:53
segment .text
global main
main:
	mov eax,3
	push eax 
	mov eax,5
	push eax 
	pop ebx
	pop eax
    add eax, ebx
	push eax
	mov eax,8
	push eax 
	pop ebx
	pop eax
	mul ebx
	pop eax
	mov eax,10
	push eax 
	mov eax,4
	push eax 
	pop ebx
	pop eax
    sub eax, ebx
	push eax
	mov eax,2
	push eax 
	pop ebx
	pop eax
	div ebx
	push eax
	pop ebx
	pop eax
    sub eax, ebx
	push eax
	pop eax
;    If
	mov eax,x26
	push eax
	mov eax,61
	push eax 
	pop ebx
	pop eax
    cmp eax, ebx
	jne brinco_if_1
	;PRINT_DEC
;Etiqueta del If
brinco_if_1:
	RET
         section .data
x26 dd 0
