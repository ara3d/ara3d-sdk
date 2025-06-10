FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install zsh and tools
RUN apt-get update && \
    apt-get install -y zsh curl git unzip && \
    chsh -s /usr/bin/zsh

# Optional: oh-my-zsh
RUN sh -c "$(curl -fsSL https://raw.githubusercontent.com/ohmyzsh/ohmyzsh/master/tools/install.sh)" || true

# Create a dev user with zsh configured
RUN useradd -ms /usr/bin/zsh devuser

# Prevent zsh from launching the config menu by creating an empty .zshrc
RUN mkdir -p /home/devuser && \
    touch /home/devuser/.zshrc && \
    chown -R devuser:devuser /home/devuser

USER devuser
WORKDIR /home/devuser/app

CMD ["zsh"]