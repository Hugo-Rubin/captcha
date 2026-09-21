function [ ce_img ] = Canny_Edge( img )
%CANNYEDGE Aplica o algoritmo de detecção de bordas de Canny

if ndims(img) == 3
    gs_img = Grayscale(img);
else
    gs_img = img;
end

ce_img = edge(gs_img, 'canny');

figure, imshow(ce_img), title('Canny Edge Detector');

end

