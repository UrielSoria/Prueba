;Fecha de Compilación: 19/03/2025
;Hora de Compilación: 10:37:38
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
	PRINT_STRING "Valor de altura = "
    GET_DEC 4,EAX
    mov dword[altura], EAX
	PRINT_DEC 4,altura
	NEWLINE
; Asignacion de x 
	mov eax,3
	push eax 
	mov eax, dword[altura]
	push eax
	pop ebx
	pop eax
;suma
    add eax, ebx
	push eax
	mov eax,8
	push eax 
	pop ebx
	pop eax
;multiplicacion
	mul ebx
	push eax
	mov eax,10
	push eax 
	mov eax,4
	push eax 
	pop ebx
	pop eax
;resta
    sub eax, ebx
	push eax
	mov eax,2
	push eax 
	pop ebx
	pop eax
;division
	div bx
	push eax
	pop ebx
	pop eax
;resta
    sub eax, ebx
	push eax
	pop eax
	mov dword[x],eax
; Decremento de x 
	dec dword [x]
	mov eax, dword[altura]
	push eax
	mov eax,8
	push eax 
	pop ebx
	pop eax
;multiplicacion
	mul ebx
	push eax
;+=
	pop eax
	add dword[x], eax
	mov eax,2
	push eax 
;*=
	pop eax
	mov ebx, [x]
	mul ebx
	push eax
	mov dword[x], eax
	mov eax, dword[y]
	push eax
	mov eax,6
	push eax 
	pop ebx
	pop eax
;resta
    sub eax, ebx
	push eax
;/=
	pop ebx
	mov eax, [x]
	div bx
	mov [x], eax
;for
; Asignacion de i 
	mov eax,1
	push eax 
	pop eax
	mov dword[i],eax
For2:
	mov eax, dword[i]
	push eax
	mov eax, dword[altura]
	push eax
	pop ebx
	pop eax
    cmp eax, ebx
	ja brinco_for_1
; Incremento de i 
	inc dword[i]
;for
; Asignacion de j 
	mov eax,1
	push eax 
	pop eax
	mov dword[j],eax
For4:
	mov eax, dword[j]
	push eax
	mov eax, dword[i]
	push eax
	pop ebx
	pop eax
    cmp eax, ebx
	ja brinco_for_3
; Incremento de j 
	inc dword[j]
;    If
	mov eax, dword[j]
	push eax
	mov eax,2
	push eax 
	pop ebx
	pop eax
;modulo
	div bx
	push edx
	mov eax,0
	push eax 
	pop ebx
	pop eax
    cmp eax, ebx
	jne brinco_if_1
	PRINT_STRING "*"
;Etiqueta del If
brinco_if_1:
; Else
	PRINT_STRING "-"
	jmp For4
brinco_for_3:
	PRINT_STRING ""
	NEWLINE
	jmp For2
brinco_for_1:
; Asignacion de i 
	mov eax,0
	push eax 
	pop eax
	mov dword[i],eax
	; Do
jmp_do_1:
	PRINT_STRING "-"
; Incremento de i 
	inc dword[i]
	mov eax, dword[i]
	push eax
	mov eax, dword[altura]
	push eax
	mov eax,2
	push eax 
	pop ebx
	pop eax
;multiplicacion
	mul ebx
	push eax
	pop ebx
	pop eax
    cmp eax, ebx
	JB jmp_do_1
	PRINT_STRING ""
	NEWLINE
;for
; Asignacion de i 
	mov eax,1
	push eax 
	pop eax
	mov dword[i],eax
For6:
	mov eax, dword[i]
	push eax
	mov eax, dword[altura]
	push eax
	pop ebx
	pop eax
    cmp eax, ebx
	ja brinco_for_5
; Incremento de i 
	inc dword[i]
; Asignacion de j 
	mov eax,1
	push eax 
	pop eax
	mov dword[j],eax
;While
While1:
	mov eax, dword[j]
	push eax
	mov eax, dword[i]
	push eax
	pop ebx
	pop eax
    cmp eax, ebx
	ja brinco_While_1
	PRINT_STRING ""
	PRINT_DEC 4,j 
; Incremento de j 
	inc dword[j]
	jmp While1
brinco_While_1:
	PRINT_STRING ""
	NEWLINE
	jmp For6
brinco_for_5:
; Asignacion de i 
	mov eax,0
	push eax 
	pop eax
	mov dword[i],eax
	; Do
jmp_do_2:
	PRINT_STRING "-"
; Incremento de i 
	inc dword[i]
	mov eax, dword[i]
	push eax
	mov eax, dword[altura]
	push eax
	mov eax,2
	push eax 
	pop ebx
	pop eax
;multiplicacion
	mul ebx
	push eax
	pop ebx
	pop eax
    cmp eax, ebx
	JB jmp_do_2
	PRINT_STRING ""
	NEWLINE
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
print dd ""
