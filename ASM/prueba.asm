;Fecha de Compilación: 12/03/2025
;Hora de Compilación: 08:52:15
segment .text
global main
main:
	mov edx, 0
	mov eax,0
	push eax 
;Asignacion de a
	pop eax
	mov dword[a], eax
;    If
	mov eax, dword[a]
	push eax
	mov eax,1
	push eax 
	pop ebx
	pop eax
    cmp eax, ebx
	jne brinco_if_1
; Asignacion de a 
	mov eax,200
	push eax 
	pop eax
	mov dword[a],eax
;Etiqueta del If
brinco_if_1:
; Else
;    If
	mov eax, dword[a]
	push eax
	mov eax,0
	push eax 
	pop ebx
	pop eax
    cmp eax, ebx
	jne brinco_if_2
; Asignacion de a 
	mov eax,300
	push eax 
	pop eax
	mov dword[a],eax
;Etiqueta del If
brinco_if_2:
	RET
         section .data
a dd 0
