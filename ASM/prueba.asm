;Fecha de Compilación: 19/03/2025
;Hora de Compilación: 11:31:03
%include "io.inc"
segment .text
global main
main:
	mov edx, 0
	mov eax,0
	push eax 
;Asignacion de x
	pop eax
	mov dword[x], eax
	mov eax,10
	push eax 
;Asignacion de y
	pop eax
	mov dword[y], eax
	mov eax,2
	push eax 
;Asignacion de z
	pop eax
	mov dword[z], eax
; Asignacion de c 
	mov eax,100
	push eax 
	mov eax,200
	push eax 
	pop ebx
	pop eax
;suma
    add eax, ebx
	push eax
	pop eax
	mov dword[c],eax
; Asignacion de altura 
	mov eax,3
	push eax 
	pop eax
	mov dword[altura],eax
;    If
	mov eax, dword[altura]
	push eax
	mov eax,3
	push eax 
	pop ebx
	pop eax
    cmp eax, ebx
	jne brinco_if_1
	PRINT_STRING "altura bien"
	NEWLINE
;Etiqueta del If
brinco_if_1:
; Else
	PRINT_STRING "altura mal"
	RET
         section .data
;int
altura dd 0
;int
i dd 0
;int
j dd 0
;int
x dd 0
;int
y dd 0
;int
z dd 0
;int
c dd 0
