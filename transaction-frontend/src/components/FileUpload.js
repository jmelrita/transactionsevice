import React, { useState } from 'react';

function FileUpload()
{
    const [file, setFile] = useState(null);
    const [message, setMessage] = useState('');

    const handleFileChange = (event) => {
        setFile(event.target.files[0]);
    };

    const handleSubmit = async(event) => {
        event.preventDefault();
        const formData = new FormData();
        formData.append('file', file);

        const response = await fetch('http://localhost:5000/api/transactions/upload', {
method: 'POST',
            body: formData,
        });

        if (response.ok)
        {
            setMessage("File uploaded successfully.");
        }
        else
        {
            const errors = await response.json();
            setMessage(errors.join('\n'));
        }
    };

    return (
        <div className="container">
            <h1>Upload Transactions</h1>
            <form onSubmit={ handleSubmit}>
                <div className="form-group">
                    <input type="file" className="form-control" onChange={ handleFileChange} />
                </div>
                <button type="submit" className="btn btn-primary">Upload</button>
            </form>
            { message && <p>{ message}</p>}
        </div>
    );
}

export default FileUpload;
