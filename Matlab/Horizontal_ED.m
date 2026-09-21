function [ he_img ] = Horizontal_ED( img )
%Horizontal_ED Aplica a detecção de arestas horizontais na imagem recebida

if ndims(img) == 3
    he_img = Grayscale(img);
else
    he_img = img;
end


he_img = he_img';

[r, c] = size(he_img);

he_img = [zeros(r, 1) he_img zeros(r, 1)];

for x = 1:r
    for y = 2:c
        he_img(x, y) = he_img(x, y + 1) - he_img(x, y);
    end
end

he_img = he_img';
   
figure, imshow(he_img), title('Horizontal Edges');
        
end


