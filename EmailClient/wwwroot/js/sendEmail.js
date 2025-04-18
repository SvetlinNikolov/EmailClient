﻿window.addEventListener('load', () => {
    console.log("sendEmail.js is loaded");

    const toggleBtn = document.getElementById('toggleComposeBtn');
    const composeContainer = document.getElementById('composeFormContainer');

    if (!toggleBtn || !composeContainer) {
        console.log("Missing elements");
        return;
    }

    toggleBtn.addEventListener('click', () => {
        console.log("Compose button clicked");
        if (composeContainer.innerHTML.trim() === '') {
            composeContainer.innerHTML = `
                <form id="sendEmailForm" class="compose-form">
                    <input type="email" name="to" placeholder="To" required><br>
                    <input type="text" name="subject" placeholder="Subject"><br>
                    <textarea name="body" placeholder="Message" rows="4"></textarea><br>
                    <button type="submit">Send</button>
                    <p id="sendStatus" class="send-status"></p>
                </form>
            `;

            const sendForm = document.getElementById('sendEmailForm');
            sendForm.addEventListener('submit', async function (e) {
                e.preventDefault(); // prevent default GET submission

                const form = e.target;
                const data = {
                    to: form.to.value,
                    subject: form.subject.value,
                    body: form.body.value
                };

                const status = document.getElementById('sendStatus');
                status.textContent = 'Sending...';

                try {
                    const res = await fetch('/email/sendemail', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(data)
                    });

                    if (!res.ok) {
                        const result = await res.json();
                        throw new Error(result.message || "Unknown error");
                    }

                    status.textContent = 'Email sent successfully.';
                    status.style.color = 'green';
                    form.reset();

                } catch (err) {
                    status.textContent = err.message;
                    status.style.color = 'red';
                }
            });
        } else {
            composeContainer.innerHTML = '';
        }
    });
});
