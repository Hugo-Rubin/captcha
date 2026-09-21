function [ gs_img ] = Grayscale( img )
%GRAYSCALE Recebe uma imagem colorida e a retorna em escala de cinza.

gs_img = rgb2gray(img);

end

