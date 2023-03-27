.data
mask_for_red dd 000FF0000h 
mask_for_green dd 00000FF00h
mask_for_blue dd 0000000FFh

.code
remove_color_asm proc
  
    movdqu xmm0, [RCX]          ;move 4 pixels from an array to xmm0

    movd xmm1, edx              ;move the 32-bit color of the lower boundary to xmm1
    pshufd xmm1, xmm1, 0        ;copy this pixel so that xmm1 contains 4 of them

    movd xmm2, r8d              ;move the 32-bit color of the upper boundary to xmm2              
    pshufd xmm2, xmm2, 0        ;copy this pixel so that xmm2 contains 4 of them

;---------------------- red section ------------------------------------------


    movd xmm11, [mask_for_red]           ;move the 32-bit mask that keeps the red channel to xmm11
    pshufd xmm11, xmm11, 0               ;copy this mask to the remaining positions in xmm11
    
    vpand xmm3, xmm1, xmm11              ; keep the red channel for the lower boundary and store it in xmm3
    vpand xmm6, xmm2, xmm11              ; keep the red channel for the upper boundary and store it in xmm6
    vpand xmm15, xmm0, xmm11             ; keep the red channel for the pixels and store it in xmm15

    vpcmpgtd xmm3, xmm15, xmm3           ; check if the red color of the pixels is above the lower boundary
    vpcmpgtd xmm6, xmm6, xmm15           ; check if the upper boundary is above the red color of the pixels

    vpand xmm7, xmm3, xmm6               ; store the final mask after the AND operation


;-----------------------------------------------------------------------------(the resulting mask is stored in xmm7)
    

;---------------------- green section ----------------------------------------


    movd xmm12, [mask_for_green]         ; move the 32-bit mask that keeps the green channel to xmm12
    pshufd xmm12, xmm12, 0               ; copy this mask to the remaining positions in xmm12
    
    vpand xmm3, xmm1, xmm12              ; keep the green channel for the lower boundary and store it in xmm3
    vpand xmm6, xmm2, xmm12              ; keep the green channel for the upper boundary and store it in xmm6
    vpand xmm15, xmm0, xmm12             ; keep the green channel for the pixels and store it in xmm15

    vpcmpgtd xmm3, xmm15, xmm3           ; check if the green color of the pixels is above the lower boundary
    vpcmpgtd xmm6, xmm6, xmm15           ; check if the upper boundary is above the green color of the pixels

    vpand xmm8, xmm3, xmm6               ; store the final mask after the AND operation


;-----------------------------------------------------------------------------(the resulting mask is stored in xmm8)


;---------------------- blue section -----------------------------------------


    movd xmm13, [mask_for_blue]          ; move the 32-bit mask that keeps the blue channel to xmm13.
    pshufd xmm13, xmm13, 0               ; copy this mask to the remaining positions in xmm13

    vpand xmm3, xmm1, xmm13              ; keep the blue channel for the lower boundary and store it in xmm3
    vpand xmm6, xmm2, xmm13              ; keep the blue channel for the upper boundary and store it in xmm6
    vpand xmm15, xmm0, xmm13             ; keep the blue channel for the pixels and store it in xmm15

    vpcmpgtd xmm3, xmm15, xmm3           ; check if the blue color of the pixels is above the lower boundary
    vpcmpgtd xmm6, xmm6, xmm15           ; check if the upper boundary is above the blue color of the pixels

    vpand xmm9, xmm3, xmm6               ; store the final mask after the AND operation

;-----------------------------------------------------------------------------(the resulting mask is stored in xmm9)



;---------------------- summary section -----------------------------------------

    vpand xmm3, xmm8, xmm7               ; perform the AND operation on the masks
    vpand xmm6, xmm9, xmm3               ; perform the AND operation on the masks and store the final result in xmm6

;--------------------------------------------------------------------------------(The final mask is stored in xmm6)
    
    
    vpcmpeqd xmm5, xmm5, xmm5            ; negate the mask
    pandn xmm6, xmm5            

    vpand xmm0, xmm0, xmm6               ; perform the AND operation on the pixels (xmm0) and the mask (xmm4), and store the result in xmm0 
    movdqu [RCX], xmm0                   ; save the modified 4 pixels back into the array.

    ret

remove_color_asm endp
end

