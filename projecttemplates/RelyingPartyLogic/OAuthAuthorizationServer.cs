﻿//-----------------------------------------------------------------------
// <copyright file="OAuthAuthorizationServer.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RelyingPartyLogic {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;

	using DotNetOpenAuth.Messaging.Bindings;
	using DotNetOpenAuth.OAuth2;

	/// <summary>
	/// Provides OAuth 2.0 authorization server information to DotNetOpenAuth.
	/// </summary>
	public class OAuthAuthorizationServer : IAuthorizationServer {
		internal static readonly RSAParameters AsymmetricKey;

		private static readonly byte[] secret;

		private readonly INonceStore nonceStore = new NonceDbStore();

		static OAuthAuthorizationServer() {
			// TODO: Replace this sample code with real code.
			// For this sample, we just generate random secrets.
			RandomNumberGenerator crypto = new RNGCryptoServiceProvider();
			secret = new byte[16];
			crypto.GetBytes(secret);

			AsymmetricKey = new RSACryptoServiceProvider().ExportParameters(true);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OAuthAuthorizationServer"/> class.
		/// </summary>
		public OAuthAuthorizationServer() {
		}

		#region IAuthorizationServer Members

		/// <summary>
		/// Gets the secret used to symmetrically encrypt and sign authorization codes and refresh tokens.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// This secret should be kept strictly confidential in the authorization server(s)
		/// and NOT shared with the resource server.  Anyone with this secret can mint
		/// tokens to essentially grant themselves access to anything they want.
		/// </remarks>
		public byte[] Secret {
			get { return secret; }
		}

		/// <summary>
		/// Gets the asymmetric private key to use for signing access tokens.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// The public key in the private/public key pair will be used by the resource
		/// servers to validate that the access token is minted by a trusted authorization server.
		/// </remarks>
		public RSAParameters AccessTokenSigningPrivateKey {
			get { return AsymmetricKey; }
		}

		/// <summary>
		/// Gets the authorization code nonce store to use to ensure that authorization codes can only be used once.
		/// </summary>
		/// <value>The authorization code nonce store.</value>
		public INonceStore VerificationCodeNonceStore {
			get { return this.nonceStore; }
		}

		/// <summary>
		/// Gets the client with a given identifier.
		/// </summary>
		/// <param name="clientIdentifier">The client identifier.</param>
		/// <returns>The client registration.  Never null.</returns>
		/// <exception cref="ArgumentException">Thrown when no client with the given identifier is registered with this authorization server.</exception>
		public IConsumerDescription GetClient(string clientIdentifier) {
			return Database.DataContext.Clients.First(c => c.ClientIdentifier == clientIdentifier);
		}

		/// <summary>
		/// Determines whether a described authorization is (still) valid.
		/// </summary>
		/// <param name="authorization">The authorization.</param>
		/// <returns>
		/// 	<c>true</c> if the original authorization is still valid; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// 	<para>When establishing that an authorization is still valid,
		/// it's very important to only match on recorded authorizations that
		/// meet these criteria:</para>
		/// 1) The client identifier matches.
		/// 2) The user account matches.
		/// 3) The scope on the recorded authorization must include all scopes in the given authorization.
		/// 4) The date the recorded authorization was issued must be <em>no later</em> that the date the given authorization was issued.
		/// <para>One possible scenario is where the user authorized a client, later revoked authorization,
		/// and even later reinstated authorization.  This subsequent recorded authorization
		/// would not satisfy requirement #4 in the above list.  This is important because the revocation
		/// the user went through should invalidate all previously issued tokens as a matter of
		/// security in the event the user was revoking access in order to sever authorization on a stolen
		/// account or piece of hardware in which the tokens were stored. </para>
		/// </remarks>
		public bool IsAuthorizationValid(DotNetOpenAuth.OAuth2.ChannelElements.IAuthorizationDescription authorization) {
			// We don't support revoking tokens yet.
			return true;
		}

		#endregion
	}
}
