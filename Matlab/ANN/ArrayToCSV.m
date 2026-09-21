function ArrayToCSV( filename, Array, delimiter, mode )
% Writes cell array content into a *.csv file. 
% 
% CELLARRAYTOCSV(filename,cellArray,delimiter) 
% 
% filename = Name of the file to save. [ i.e. 'text.csv' ] 
% array = Name of the array where the data is in 
% delimiter = seperating sign, normally:',' (it's default) 
% mode = specifies the mode of opening the file. See fopen() for a detailed 
% list (default is overwrite i.e. 'w') 

if nargin<3 
    delimiter = ','; 
end 
if nargin<4 
    mode = 'w'; 
end

data = fopen(filename, mode);

for i = 1 : size(Array, 1) 
    for j = 1 : size(Array, 2) 
         
        var = Array(i, j); 
         
        if size(var, 1) == 0 
            var = ''; 
        end 
         
        if isnumeric(var) == 1 
            var = num2str(var); 
        end 
         
        fprintf(data, var); 
         
        if j ~= size(Array, 2) 
            fprintf(data, delimiter); 
        end 
    end 
    fprintf(data, '\n'); 
end 

fclose(data);

end


