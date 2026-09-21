function [ C ] = Mult( X, T )
%MULT Summary of this function goes here
%   Detailed explanation goes here

%display(X(1, 1:100)');
%display(T(1:100, 1:100));
%pause;

 %fid = fopen('D:\Arquivos e Pastas\Captcha\auto\tratadas\Cmatlab2.txt', 'w');

C = zeros(size(X,1), size(T,2));
    for i = 1 : size(X,1)
       for j = 1 : size(T,2)
           for k = 1 : size(T,1)
               C(i, j) = X(i, k) * T(k, j) + C(i, j);
              % if (j == 2)
               %    fclose(fid);
                %   pause;
               %end
               
                
%                fprintf(fid, '%.9f\n', C(i,j));
        
               
               %display(k);   
               %display(X(i, k));
               %display(C(i, j));
               
               
           end
       end
    end
    

end

