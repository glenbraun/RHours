module SignContribution

open RHours.Data
open JsonSerialization

open RHours.Crypto

open System
open System.IO
open System.Text
open Json

let private GetJsonBytes (data: obj) : byte[] =
    use m = new MemoryStream()
    use sw = new StreamWriter(m, Encoding.UTF8)
    
    let json = Serialize data
    WriteJson sw json
    sw.Flush()
    m.Flush()

    let jsonBytes = m.ToArray()
    jsonBytes

// Done by the contributor
let SignCompensationInvoice (invoice: CompensationInvoice) (publicKey: string) (privateKey: string) = 
    //let (publicKeyText, privateKeyText) = Ed25519Signature.CreateKeyPair()

    let jsonBytes = GetJsonBytes invoice
    let hashBytes = CryptoProvider.Hash(jsonBytes)

    let pk = sprintf "%s\n%s\n%s" "-----BEGIN PRIVATE KEY-----" privateKey "-----END PRIVATE KEY-----"
    let signedBytes = CryptoProvider.Sign(pk, hashBytes)

    let signedProposal = 
        {
            Invoice = invoice;
            InvoiceHash = hashBytes;
            ContributorSignature = signedBytes;
            ContributorPublicKey = publicKey;
        }

    signedProposal

// Done by the project leader
let SignCompensationProposal (proposal: CompensationProposal) (publicKey: string) (privateKey: string) = 
    let jsonBytes = GetJsonBytes proposal
    let hashBytes = CryptoProvider.Hash(jsonBytes)

    let pk = sprintf "%s\n%s\n%s" "-----BEGIN PRIVATE KEY-----" privateKey "-----END PRIVATE KEY-----"
    let signedBytes = CryptoProvider.Sign(pk, hashBytes)

    let signedAgreement = 
        {
            Proposal = proposal;
            ProposalHash = hashBytes;           // Hash of Proposal
            AcceptorSignature = signedBytes;    // Signature of AcceptanceHash using Acceptor Key
            AcceptorPublicKey = publicKey;
        }

    signedAgreement

// Verify ContributorSignedCompensationAgreement
let VerifyCompensationProposal (proposal: CompensationProposal) =
    let pk = sprintf "%s\n%s\n%s" "-----BEGIN PUBLIC KEY-----" (proposal.ContributorPublicKey) "-----END PUBLIC KEY-----"
    let verified = CryptoProvider.Verify(pk, proposal.InvoiceHash, proposal.ContributorSignature)
    verified

// Verify AcceptedCompensationAgreement
let VerifyCompensationAgreement (agreement: CompensationAgreement) =
    let pk = sprintf "%s\n%s\n%s" "-----BEGIN PUBLIC KEY-----" (agreement.AcceptorPublicKey) "-----END PUBLIC KEY-----"
    let verified = CryptoProvider.Verify(pk, agreement.ProposalHash, agreement.AcceptorSignature)
    verified
